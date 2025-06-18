using System;
using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class SearchSimilarDocumentsQueryHandler(
    IEmbeddingService embeddingService,
    IVectorDatabaseService vectorDatabaseService,
    IIaService iaService,
    ILogger<SearchSimilarDocumentsQueryHandler> logger
) : IRequestHandler<SearchSimilarDocumentsQuery, string>
{
    private readonly IEmbeddingService _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
    private readonly IVectorDatabaseService _vectorDatabaseService = vectorDatabaseService ?? throw new ArgumentNullException(nameof(vectorDatabaseService));
    private readonly IIaService _iaService = iaService ?? throw new ArgumentNullException(nameof(iaService));
    private readonly ILogger<SearchSimilarDocumentsQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<string> Handle(SearchSimilarDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Handling SearchSimilarDocumentsQuery for: {Query}", request?.SearchQuery);

        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
        {
            _logger?.LogWarning("Search query is null or empty.");
            throw new ArgumentException("Search query cannot be null or empty.", nameof(request.SearchQuery));
        }

        // Etapa 1 - Gera o embedding
        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.SearchQuery);

        // Etapa 2 - Busca documentos similares
        var similarDocuments = await _vectorDatabaseService.SearchSimilarAsync(embedding);
        var documentContents = similarDocuments
            .Select(d => d.Text)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .ToList();

        _logger?.LogInformation("Found {Count} similar documents for query: {Query}", documentContents.Count, request.SearchQuery);

        if (documentContents.Count == 0)
        {
            _logger?.LogWarning("No documents found for query: {Query}", request.SearchQuery);
            return "Nenhum documento relevante encontrado.";
        }

        // Etapa 3 - Gera resposta com RAG
        var answer = await _iaService.GenerateAnswerAsync(request.SearchQuery, documentContents);

        _logger?.LogInformation("Generated answer successfully.");

        return answer;
    }
}
