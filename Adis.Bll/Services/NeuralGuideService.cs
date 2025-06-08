using Adis.Bll.Configurations;
using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using AutoMapper;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers.Ollama;
using LangChain.Splitters.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.Text;
using System.Text.RegularExpressions;

namespace Adis.Bll.Services
{
    public class NeuralGuideService : INeuralGuideService
    {
        private const string CacheKey = "documents";
        private const string CollectionName = "legal_docs";
        private const int EmbeddingDimensions = 768;
        private const int BatchSize = 100;
        private const int ChunkSize = 512;
        private const int ChunkOverlap = 128;

        private static readonly Regex CleanTextRegex = new Regex(
            @"[^\w\sа-яА-ЯёЁ.,;:!?()-]",
            RegexOptions.Compiled | RegexOptions.CultureInvariant,
            TimeSpan.FromMilliseconds(100));

        private readonly InMemoryVectorDatabase _vectorDatabase;
        private readonly OllamaEmbeddingModel _embeddingModel;
        private readonly OllamaChatModel _llm;
        private readonly IMemoryCache _cache;
        private readonly IDocumentRepository _documentRepository;
        private readonly IMapper _mapper;
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private readonly char[] _keywordSeparators = { ' ', ',', '.', '?' };

        private List<Document> _allDocuments = new();
        private ILookup<string, Document> _keywordIndex;
        private IVectorCollection _collection;

        public NeuralGuideService(
            IMemoryCache cache,
            IOptions<OllamaSetting> ollamaSetting,
            IDocumentRepository documentRepository,
            IMapper mapper)
        {
            _cache = cache;
            _documentRepository = documentRepository;
            _mapper = mapper;
            var settings = ollamaSetting.Value;
            var provider = new OllamaProvider(settings.OllamaUrl);

            _embeddingModel = new OllamaEmbeddingModel(provider, settings.EmbeddingModel);
            _llm = new OllamaChatModel(provider, settings.LlmModel);
            _vectorDatabase = new InMemoryVectorDatabase();
        }

        public async Task InitializeAsync(IEnumerable<DocumentDto>? documentsDtos = null, string? directoryPath = null)
        {
            if (_cache.TryGetValue(CacheKey, out (List<Document> Docs, IVectorCollection Collection) cacheEntry))
            {
                _allDocuments = cacheEntry.Docs;
                _collection = cacheEntry.Collection;
                BuildKeywordIndex();
                return;
            }

            await _initLock.WaitAsync();
            try
            {
                // Double-check cache after acquiring lock
                if (_cache.TryGetValue(CacheKey, out cacheEntry))
                {
                    _allDocuments = cacheEntry.Docs;
                    _collection = cacheEntry.Collection;
                    BuildKeywordIndex();
                    return;
                }

                if (documentsDtos == null || directoryPath == null)
                {
                    var dbDocs = await _documentRepository.GetGuideDocumentsAsync();
                    documentsDtos = _mapper.Map<IEnumerable<DocumentDto>>(dbDocs);
                    directoryPath = "documents";

                    if (!documentsDtos.Any())
                        return;
                }

                var documents = new ConcurrentBag<Document>();
                var loadTasks = new List<Task>();

                foreach (var docDto in documentsDtos)
                {
                    var filePath = $"{directoryPath}/{docDto.IdDocument}{Path.GetExtension(docDto.FileName).ToLowerInvariant()}";
                    if (!File.Exists(filePath)) continue;

                    loadTasks.Add(Task.Run(async () =>
                    {
                        var loader = GetDocumentLoader(filePath);
                        var loaded = await loader.LoadAsync(DataSource.FromPath(filePath));

                        foreach (var doc in loaded)
                        {
                            var cleanedContent = CleanLegalText(doc.PageContent);
                            documents.Add(new Document(
                                cleanedContent,
                                new Dictionary<string, object>
                                {
                                    ["source"] = docDto.FileName,
                                    ["page"] = "auto",
                                    ["score"] = 0.0,
                                    ["idDocument"] = docDto.IdDocument
                                }));
                        }
                    }));
                }

                await Task.WhenAll(loadTasks);
                _allDocuments = documents.ToList();

                BuildKeywordIndex();

                var splitter = new RecursiveCharacterTextSplitter(
                    chunkSize: ChunkSize,
                    chunkOverlap: ChunkOverlap,
                    separators: new[] { "\n\n", "\n", ". ", "! ", "? ", "; ", ", ", " " },
                    lengthFunction: text => text.Split().Length);

                var splitDocuments = splitter.SplitDocuments(_allDocuments);

                await _vectorDatabase.CreateCollectionAsync(CollectionName, EmbeddingDimensions);
                _collection = await _vectorDatabase.GetCollectionAsync(CollectionName);

                var batchTasks = new List<Task>();
                for (int i = 0; i < splitDocuments.Count; i += BatchSize)
                {
                    var batch = splitDocuments.Skip(i).Take(BatchSize).ToList();
                    batchTasks.Add(_collection.AddSplitDocumentsAsync(_embeddingModel, batch));
                }

                await Task.WhenAll(batchTasks);
                _cache.Set(CacheKey, (_allDocuments, _collection), TimeSpan.FromHours(24));
            }
            finally
            {
                _initLock.Release();
            }
        }

        public async Task<string> SendRequestForGuideAsync(string request)
        {
            await InitializeAsync();

            var semanticTask = _collection.GetSimilarDocuments(
                _embeddingModel,
                request,
                amount: 5,
                scoreThreshold: 0.7f);

            var keywordResults = KeywordSearch(request, 3);
            var semanticResults = await semanticTask;
            var finalResults = MergeResults(semanticResults.ToList(), keywordResults);

            var context = BuildContext(finalResults);
            var response = await GetLlmResponseAsync(context, request);

            return response;
        }

        private void BuildKeywordIndex()
        {
            _keywordIndex = _allDocuments
                .SelectMany(doc =>
                    doc.PageContent.Split(_keywordSeparators, StringSplitOptions.RemoveEmptyEntries)
                        .Select(term => (term.ToLowerInvariant(), doc)))
                .ToLookup(pair => pair.Item1, pair => pair.doc);
        }

        private List<Document> KeywordSearch(string query, int topK)
        {
            var terms = query.Split(_keywordSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToLowerInvariant());

            var docScores = new Dictionary<Document, int>();

            foreach (var term in terms)
            {
                foreach (var doc in _keywordIndex[term])
                {
                    docScores.TryGetValue(doc, out var current);
                    docScores[doc] = current + 1;
                }
            }

            return docScores
                .OrderByDescending(kv => kv.Value)
                .Take(topK)
                .Select(kv => kv.Key)
                .ToList();
        }

        private List<Document> MergeResults(List<Document> semantic, List<Document> keyword)
        {
            return semantic.Concat(keyword)
                .DistinctBy(d => d.PageContent)
                .OrderByDescending(GetDocumentScore)
                .ToList();
        }

        private string BuildContext(IEnumerable<Document> documents)
        {
            var contextBuilder = new StringBuilder(4096);

            foreach (var doc in documents.Where(d => !string.IsNullOrWhiteSpace(d.PageContent)))
            {
                contextBuilder.AppendLine(doc.PageContent);

                if (doc.Metadata.TryGetValue("source", out var source) &&
                    doc.Metadata.TryGetValue("idDocument", out var idDocument))
                {
                    var fileName = Path.GetFileNameWithoutExtension(source.ToString()!);
                    contextBuilder.AppendLine($"[Источник: {fileName}|id:{idDocument}]");
                }
                contextBuilder.AppendLine();
            }

            return contextBuilder.ToString();
        }

        private async Task<string> GetLlmResponseAsync(string context, string question)
        {
            var responseBuilder = new StringBuilder();
            var prompt = BuildPrompt(context, question);

            await foreach (var responsePart in _llm.GenerateAsync(prompt))
            {
                responseBuilder.Append(responsePart);
            }

            return responseBuilder.ToString();
        }

        private static string BuildPrompt(string context, string question)
        {
            return $"""
                [INST] <<SYS>>
                Ты юридический ассистент, работающий с нормативными документами. Строго придерживайся следующих правил:

                1. Формат ответа:
                ### Вывод
                [Краткий итоговый ответ на вопрос]

                ### Обоснование
                [Подробное объяснение с ссылками на конкретные пункты документов]
    
                ### Источники
                [Нумерованный список использованных документов в формате:
                1. [Источник: Имя Файла|id:ID документа]
                ]

                2. Используй только предоставленный контекст
                3. Если информации нет - явно укажи на это
                4. Для ссылок используй только данные из поля "Источник"
                5. Сохраняй структуру документа (заголовки, списки, код)
                6. Источники обязательно оформляется в квадратных скобках как в контексте

                Контекст:
                {context}
                <</SYS>>

                Вопрос: {question} 
                [/INST]
                """;
        }

        private static IDocumentLoader GetDocumentLoader(string filePath)
        {
            return Path.GetExtension(filePath).ToLower() switch
            {
                ".pdf" => new PdfPigPdfLoader(),
                ".docx" => new WordLoader(),
                _ => throw new NotSupportedException($"Unsupported format: {filePath}")
            };
        }

        private static string CleanLegalText(string text)
        {
            text = CleanTextRegex.Replace(text, " ");
            return text
                .Replace("\u00a0", " ", StringComparison.Ordinal)
                .Trim();
        }

        private double GetDocumentScore(Document doc)
        {
            double score = doc.Metadata.TryGetValue("score", out var scoreObj)
                ? Convert.ToDouble(scoreObj)
                : 0.0;

            double importance = doc.Metadata.TryGetValue("importance", out var importanceObj)
                ? Convert.ToDouble(importanceObj)
                : 1.0;

            return 0.6 * score + 0.4 * importance;
        }
    }
}