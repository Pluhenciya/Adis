using Adis.Bll.Interfaces;
using LangChain.Databases.InMemory;
using LangChain.DocumentLoaders;
using LangChain.Extensions;
using LangChain.Providers;
using LangChain.Providers.Ollama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class NeuralGuideService : INeuralGuideService
    {
        public async Task<string> SendRequestForGuideAsync(string request)
        {
            var provider = new OllamaProvider();
            var embeddingModel = new OllamaEmbeddingModel(provider, "nomic-embed-text");
            var llm = new OllamaChatModel(provider, "gemma3");

            // Загрузка документов
            var vectorDatabase = new InMemoryVectorDatabase();
            var vectorCollection = await vectorDatabase.AddDocumentsFromAsync<PdfPigPdfLoader>(
                embeddingModel,
                dimensions: 768, 
                dataSource: DataSource.FromPath("договор.pdf"),
                collectionName: "legal_docs");

            var similarDocuments = await vectorCollection.GetSimilarDocuments(embeddingModel, request, amount: 5);

            var answer = await llm.GenerateAsync(
                $"""
                 Ответь на вопрос на основе контекста. Если ответа нет, скажи 'Не знаю'.
     
                 Контекст: {similarDocuments.AsString()}
     
                 Вопрос: {request}
                 Ответ:
                 """);

            return answer;
        }
    }
}
