using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Adis.Bll.Interfaces;
using LangChain.Databases;
using LangChain.Databases.InMemory;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers.Ollama;
using LangChain.Splitters.Text;
using Microsoft.Extensions.Caching.Memory;

namespace Adis.Bll.Services
{
    public class NeuralGuideService : INeuralGuideService
    {
        private readonly IDocumentService _documentService;
        private readonly InMemoryVectorDatabase _vectorDatabase;
        private readonly OllamaEmbeddingModel _embeddingModel;
        private readonly OllamaChatModel _llm;
        private List<Document> _allDocuments = new();
        private IVectorCollection _collection;
        private readonly IMemoryCache _cache;

        public NeuralGuideService(IDocumentService documentService, IMemoryCache cache)
        {
            _documentService = documentService;
            _cache = cache;
            var ollamaProvider = new OllamaProvider();

            _embeddingModel = new OllamaEmbeddingModel(ollamaProvider, "nomic-embed-text");
            _llm = new OllamaChatModel(ollamaProvider, "gemma3");
            _vectorDatabase = new InMemoryVectorDatabase();
        }

        private async Task InitializeAsync()
        {
            var currentVersion = _documentService.GetCurrentGuideDocumentsVersion();
            var cacheKey = $"documents_{currentVersion}";

            // Проверяем кэш для документов и векторной базы
            if (_cache.TryGetValue(cacheKey, out (List<Document> Docs, IVectorCollection Collection) cacheEntry))
            {
                _allDocuments = cacheEntry.Docs;
                _collection = cacheEntry.Collection;
                return;
            }

            var documentsDtos = await _documentService.GetGuideDocumentsAsync();
            var documents = new List<Document>();

            foreach (var documentsDto in documentsDtos)
            {
                var filePath = _documentService.GetFilePathByDocument(documentsDto);
                var loader = GetDocumentLoader(filePath);
                var loaded = await loader.LoadAsync(DataSource.FromPath(filePath));

                var processed = loaded.Select(doc => new Document(
                    CleanLegalText(doc.PageContent),
                    new Dictionary<string, object>
                    {
                        ["source"] = documentsDto.FileName,
                        ["page"] = "auto",
                        ["score"] = 0.0,
                        ["idDocument"] = documentsDto.IdDocument
                    })).ToList();

                documents.AddRange(processed);
            }

            _allDocuments = documents;

            var splitter = new RecursiveCharacterTextSplitter(
                chunkSize: 512,
                chunkOverlap: 128,
                separators: new[] { "\n\n", "\n", ". ", "! ", "? ", "; ", ", ", " " },
                lengthFunction: text => text.Split().Length);

            var splitDocuments = splitter.SplitDocuments(documents);

            await _vectorDatabase.CreateCollectionAsync("legal_docs", 768);
            _collection = await _vectorDatabase.GetCollectionAsync("legal_docs");

            var batchSize = 50;
            for (int i = 0; i < splitDocuments.Count; i += batchSize)
            {
                var batch = splitDocuments.Skip(i).Take(batchSize).ToList();
                await _collection.AddSplitDocumentsAsync(_embeddingModel, batch);
            }

            // Сохраняем в кэш и документы и коллекцию
            _cache.Set(cacheKey, (_allDocuments, _collection), TimeSpan.FromHours(1));
        }

        public async Task<string> SendRequestForGuideAsync(string request)
        {
            await InitializeAsync();

            var semanticResults = await _collection.GetSimilarDocuments(
                _embeddingModel,
                request,
                amount: 5,
                scoreThreshold: 0.7f);

            var keywordResults = KeywordSearch(request, 3);
            var finalResults = MergeResults(semanticResults.ToList(), keywordResults);

            // Формируем контекст вручную
            var contextBuilder = new StringBuilder();
            foreach (var doc in finalResults.Where(d => !string.IsNullOrWhiteSpace(d.PageContent)))
            {
                contextBuilder.AppendLine(doc.PageContent);
                if (doc.Metadata.TryGetValue("source", out var source) && doc.Metadata.TryGetValue("idDocument", out var idDocument))
                {
                    var fileName = Path.GetFileNameWithoutExtension(source.ToString());
                    contextBuilder.AppendLine($"[Источник: {fileName}|id:{idDocument}]");
                }
                contextBuilder.AppendLine();
            }
            var context = contextBuilder.ToString();

            // Обрабатываем асинхронный поток ответов
            var responseStream = _llm.GenerateAsync(BuildPrompt(context, request));
            var responseBuilder = new StringBuilder();

            await foreach (var responsePart in responseStream)
            {
                responseBuilder.Append(responsePart);
            }

            return responseBuilder.ToString();
        }

        private List<Document> KeywordSearch(string query, int topK)
        {
            var terms = query.Split(new[] { ' ', ',', '.', '?' }, StringSplitOptions.RemoveEmptyEntries);

            return _allDocuments
                .AsParallel()
                .Select(doc => new
                {
                    Doc = doc,
                    Score = terms.Count(term =>
                        doc.PageContent.Contains(term, StringComparison.OrdinalIgnoreCase))
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .Select(x => x.Doc)
                .ToList();
        }

        private List<Document> MergeResults(List<Document> semantic, List<Document> keyword)
        {
            return semantic.Concat(keyword)
                .GroupBy(d => d.PageContent)
                .Select(g => g.First())
                .OrderByDescending(d => GetDocumentScore(d))  // Используем метод получения score
                .ToList();
        }

        private double GetDocumentScore(Document doc)
        {
            var score = doc.Metadata.TryGetValue("score", out var scoreObj)
                ? Convert.ToDouble(scoreObj)
                : 0.0;

            var importance = doc.Metadata.TryGetValue("importance", out var importanceObj)
                ? Convert.ToDouble(importanceObj)
                : 1.0;

            return 0.6 * score + 0.4 * importance;
        }

        private string BuildPrompt(string context, string question)
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

                Контекст:
                {context}
                <</SYS>>

                Вопрос: {question} 
                [/INST]
                """;
        }

        private static IDocumentLoader GetDocumentLoader(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLower();
            return extension switch
            {
                ".pdf" => new PdfPigPdfLoader(),
                ".docx" => new WordLoader(),
                _ => throw new NotSupportedException($"Unsupported format: {extension}")
            };
        }

        private string CleanLegalText(string text)
        {
            text = Regex.Replace(text, @"[^\w\sа-яА-ЯёЁ.,;:!?()-]", " ");
            return text
                .Replace("\u00a0", " ")
                .Trim();
        }
    }
}