using System;
using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class SearchSimilarDocumentsQueryHandler(IEmbeddingService embeddingService, IVectorDatabaseService vectorDatabaseService, ILogger<SearchSimilarDocumentsQueryHandler> logger) : IRequestHandler<SearchSimilarDocumentsQuery, IEnumerable<DocumentResposeDto>?>
{
    private readonly IEmbeddingService _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
    private readonly IVectorDatabaseService _vectorDatabaseService = vectorDatabaseService ?? throw new ArgumentNullException(nameof(vectorDatabaseService));
    private readonly ILogger<SearchSimilarDocumentsQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<IEnumerable<DocumentResposeDto>?> Handle(SearchSimilarDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Handling SearchSimilarDocumentsQuery for: {Query}", request?.SearchQuery);

        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            _logger?.LogWarning("Search query is null or empty.");

            throw new ArgumentException("Search query cannot be null or empty.", nameof(request.SearchQuery));
        }

        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.SearchQuery);

        var similarDocuments = await _vectorDatabaseService.SearchSimilarAsync(embedding);

        _logger?.LogInformation("Found {Count} similar documents for query: {Query}", similarDocuments?.Count() ?? 0, request.SearchQuery);

        return similarDocuments;
    }
}
