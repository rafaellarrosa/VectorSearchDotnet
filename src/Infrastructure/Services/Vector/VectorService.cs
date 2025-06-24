using System;
using Application.DTOs;
using Application.Interfaces;
using Infrastructure.DTOs.Vector;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Vector;

public class VectorService : IVectorService
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorDatabaseService _vectorDatabaseService;
    private readonly VectorSearchOptions _options;
    private readonly ILogger<VectorService> _logger;

    public VectorService(
        IEmbeddingService embeddingService,
        IVectorDatabaseService vectorDatabaseService,
        IOptions<VectorSearchOptions> options,
        ILogger<VectorService> logger)
    {
        _embeddingService = embeddingService;
        _vectorDatabaseService = vectorDatabaseService;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<Guid> IndexAsync(string text, Guid? externalId = null)
    {
        _logger.LogInformation("Starting indexing for text: {Text}", text);

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

        if (embedding == null || embedding.Length == 0)
            throw new InvalidOperationException("Failed to generate embedding.");

        var documentId = externalId ?? Guid.NewGuid();

        var documentDto = new DocumentDto(documentId, text, embedding);

        await _vectorDatabaseService.IndexDocumentAsync(documentDto);

        _logger.LogInformation("Document indexed successfully with Id: {Id}", documentId);

        return documentId;
    }

    public async Task<List<DocumentResposeDto>> SearchAsync(string query, float? thresholdOverride = null)
    {
        _logger.LogInformation("Vector search started for query: {Query}", query);

        var embedding = await _embeddingService.GenerateEmbeddingAsync(query);

        var rawResults = await _vectorDatabaseService.SearchSimilarAsync(
            embedding,
            topK: _options.TopK
        );

        float threshold = thresholdOverride ?? _options.DefaultSimilarityThreshold;

        var filteredResults = rawResults
            .Where(r => r.Score >= threshold)
            .OrderByDescending(r => r.Score)
            .ToList();

        _logger.LogInformation("Vector search completed. {Count} results above threshold {Threshold}.", filteredResults.Count, threshold);

        return filteredResults;
    }
}

