using System;
using Application.DTOs;
using Application.Interfaces;
using Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Document;

public class DocumentRetrievalService(IVectorService vectorService, IGraphDatabaseService graphDatabaseService, ILogger<DocumentRetrievalService> logger) : IDocumentRetrievalService
{
    private readonly IVectorService _vectorService = vectorService;
    private readonly IGraphDatabaseService _graphDatabaseService = graphDatabaseService;
    private readonly ILogger<DocumentRetrievalService> _logger = logger;

    public async Task<Guid> IndexDocumentAsync(string text, string title)
    {
        _logger.LogInformation("Starting full indexing for document.");

        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(text));

        var documentId = await _vectorService.IndexAsync(text);

        await _graphDatabaseService.CreateNodeAsync("Document", new Dictionary<string, object>
        {
            { "Id", $"{documentId}" },
            { "Title", title },
            { "Content", text }
        });

        _logger.LogInformation("Document indexed and stored in graph successfully: {Id}", documentId);

        return documentId;
    }

    public async Task<List<EnrichedDocumentDto>> RetrieveDocumentsAsync(string query, float? thresholdOverride = null)
    {
        var similarDocuments = await _vectorService.SearchAsync(query, thresholdOverride);

        var enrichedDocuments = new List<EnrichedDocumentDto>();

        foreach (var doc in similarDocuments)
        {
            var cypher = "MATCH (n:Document {Id: $id}) RETURN n.Title AS Title, n.Content AS Content";
            var parameters = new Dictionary<string, object> { { "id", $"{doc.Id}" } };
            var result = await _graphDatabaseService.QueryAsync(cypher, parameters);

            if (result.Any())
            {
                var record = result.First();
                var title = record["Title"]?.ToString() ?? string.Empty;
                var content = record["Content"]?.ToString() ?? string.Empty;

                enrichedDocuments.Add(new EnrichedDocumentDto
                {
                    Id = doc.Id,
                    Title = title,
                    Content = content,
                    Score = doc.Score
                });
            }
        }

        return enrichedDocuments;
    }
}
