using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
    IGraphDatabaseService graphDatabaseService,
    ILogger<SearchSimilarDocumentsQueryHandler> logger
) : IRequestHandler<SearchSimilarDocumentsQuery, string>
{
    private readonly IEmbeddingService _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
    private readonly IVectorDatabaseService _vectorDatabaseService = vectorDatabaseService ?? throw new ArgumentNullException(nameof(vectorDatabaseService));
    private readonly IIaService _iaService = iaService ?? throw new ArgumentNullException(nameof(iaService));
    private readonly IGraphDatabaseService _graphDatabaseService = graphDatabaseService ?? throw new ArgumentNullException(nameof(graphDatabaseService));
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

        // Etapa 2 - Busca documentos similares no Qdrant
        var similarDocuments = await _vectorDatabaseService.SearchSimilarAsync(embedding);
        var documentIds = similarDocuments.Select(d => d.Id).ToList();

        _logger?.LogInformation("Found {Count} similar documents for query: {Query}", documentIds.Count, request.SearchQuery);

        if (documentIds.Count == 0)
        {
            _logger?.LogWarning("No documents found for query: {Query}", request.SearchQuery);
            return "Nenhum documento relevante encontrado.";
        }

        // Etapa 3 - Enriquecimento via Neo4j
        var enrichedDocuments = new List<string>();

        foreach (var id in documentIds)
        {
            var cypher = "MATCH (n:Document {Id: $id}) RETURN n.Title AS Title, n.Content AS Content";
            var parameters = new Dictionary<string, object> { { "id", $"{id}" } };
            var result = await _graphDatabaseService.QueryAsync(cypher, parameters);

            if (result.Any())
            {
                var record = result.First();
                var title = record["Title"]?.ToString() ?? string.Empty;
                var content = record["Content"]?.ToString() ?? string.Empty;
                enrichedDocuments.Add($"Título: {title}\nConteúdo: {content}");
            }
        }

        if (enrichedDocuments.Count == 0)
        {
            _logger?.LogWarning("No enriched documents found for query: {Query}", request.SearchQuery);
            return "Nenhum documento relevante encontrado.";
        }

        // Etapa 4 - Geração de resposta RAG com os documentos enriquecidos
        var answer = await _iaService.GenerateAnswerAsync(request.SearchQuery, enrichedDocuments);

        _logger?.LogInformation("Generated answer successfully.");

        return answer;
    }
}
