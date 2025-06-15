using System;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases;

    public class SearchSimilarDocumentsUseCase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IVectorDatabaseService _vectorDbService;

        public SearchSimilarDocumentsUseCase(IEmbeddingService embeddingService, IVectorDatabaseService vectorDbService)
        {
            _embeddingService = embeddingService;
            _vectorDbService = vectorDbService;
        }

        public async Task<List<Document>> ExecuteAsync(string query, int topK = 5)
        {
            var embedding = await _embeddingService.GenerateEmbeddingAsync(query);
            return await _vectorDbService.SearchSimilarAsync(embedding, topK);
        }
    }