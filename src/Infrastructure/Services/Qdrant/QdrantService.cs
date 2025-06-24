using Application.DTOs;
using Infrastructure.DTOs.Qdrant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using Infrastructure.Configurations;
using Infrastructure.Interfaces;

namespace Infrastructure.Services.Qdrant;

public class QdrantService(HttpClient httpClient, IOptions<QdrantOptions> options, ILogger<QdrantService> logger, IConfiguration configuration) : IVectorDatabaseService
{
    private readonly QdrantOptions _options = options.Value;

    public async Task<Guid> IndexDocumentAsync(DocumentDto doc)
    {
        var baseUrl = configuration["QdrantOptions:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException("QdrantOptions:BaseUrl is not configured");

        var point = new QdrantPointDto
        {
            Id = $"{doc.Id}",
            Vector = doc.Embedding,
            Payload = new QdrantPayloadDto { Text = doc.Text }
        };

        var request = new QdrantPointsRequestDto
        {
            Points = [point]
        };

        var url = $"{baseUrl.TrimEnd('/')}/collections/{_options.CollectionName}/points?wait=true";

        logger?.LogDebug("Indexing document {Id} in Qdrant: {Request}", doc.Id, System.Text.Json.JsonSerializer.Serialize(request));

        var response = await httpClient.PutAsJsonAsync(url, request);

        response.EnsureSuccessStatusCode();

        return doc.Id;
    }

    public async Task<List<DocumentResposeDto>> SearchSimilarAsync(float[] embedding, int topK = 5)
    {
        var baseUrl = configuration["QdrantOptions:BaseUrl"];
        if (string.IsNullOrEmpty(baseUrl))
            throw new InvalidOperationException("QdrantOptions:BaseUrl is not configured");

        var request = new QdrantSearchRequestDto
        {
            Vector = embedding,
            Top = topK
        };

        var url = $"{baseUrl.TrimEnd('/')}/collections/{_options.CollectionName}/points/search";

        logger?.LogDebug("Searching similar documents in Qdrant: {Request}", System.Text.Json.JsonSerializer.Serialize(request));

        var response = await httpClient.PostAsJsonAsync(url, request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<QdrantSearchResponseDto>();

        logger?.LogInformation("Search completed in Qdrant. Found {Count} results.", result?.Result.Count ?? 0);

        return result?.Result.Select(r => new DocumentResposeDto(
            Guid.Parse(r.Id),
            r.Payload?.Text ?? string.Empty,
            r.Score
        )).ToList() ?? [];
    }
}
