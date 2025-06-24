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
    IIaService iaService,
    IDocumentRetrievalService documentRetrievalService,
    ILogger<SearchSimilarDocumentsQueryHandler> logger
) : IRequestHandler<SearchSimilarDocumentsQuery, string>
{
    private readonly IDocumentRetrievalService _documentRetrievalService = documentRetrievalService ?? throw new ArgumentNullException(nameof(documentRetrievalService));
    private readonly IIaService _iaService = iaService ?? throw new ArgumentNullException(nameof(iaService));
    private readonly ILogger<SearchSimilarDocumentsQueryHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<string> Handle(SearchSimilarDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Handling SearchSimilarDocumentsQuery for: {Query}", request?.SearchQuery);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.SearchQuery))
            throw new ArgumentException("Search query cannot be null or empty.", nameof(request.SearchQuery));

        var enrichedDocuments = await _documentRetrievalService.RetrieveDocumentsAsync(request.SearchQuery, 0.65f);

        if (enrichedDocuments.Count == 0)
        {
            _logger?.LogWarning("No enriched documents found for query: {Query}", request.SearchQuery);
            return "Nenhum documento relevante encontrado.";
        }

        var documentContents = enrichedDocuments.Select(doc => $"Título: {doc.Title}\nConteúdo: {doc.Content}").ToList();
        var answer = await _iaService.GenerateAnswerAsync(request.SearchQuery, documentContents);

        _logger?.LogInformation("Generated answer successfully.");
        return answer;
    }
}
