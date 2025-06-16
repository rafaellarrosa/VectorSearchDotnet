using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Infrastructure.Clients;
using Infrastructure.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure.Services.Qdrant
{
    public partial class QdrantService(IQdrantApi qdrantApi, IOptions<QdrantOptions> options, ILogger<QdrantService> logger) : IVectorDatabaseService
    {
        private readonly QdrantOptions _options = options.Value;
        private readonly IQdrantApi _qdrantApi = qdrantApi;
        private readonly ILogger<QdrantService> _logger = logger;

        public async Task CreateCollectionAsync(int vectorSize, string collectionName = null!, string distance = "Cosine")
        {
            var request = new QdrantCreateCollectionRequestDto
            {
                vectors = new VectorsConfig
                {
                    size = vectorSize,
                    distance = distance
                }
            };

            _logger?.LogInformation("Creating Qdrant collection {Collection} with vector size {VectorSize} and distance {Distance}", collectionName ?? _options.CollectionName, vectorSize, distance);

            await _qdrantApi.CreateCollectionAsync(collectionName ?? _options.CollectionName, request);

            _logger?.LogInformation("Collection {Collection} created successfully.", collectionName ?? _options.CollectionName);
        }

        public async Task IndexDocumentAsync(DocumentDto doc)
        {
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

            var strRequest = System.Text.Json.JsonSerializer.Serialize(request);

            _logger?.LogDebug("Indexing document {Id} in Qdrant: {Request}", doc.Id, strRequest);

            try
            {
                await _qdrantApi.AddPointsAsync(_options.CollectionName, request);
            }
            catch (ApiException ex)
            {
                _logger?.LogError(ex, "Failed to index document {Id} in Qdrant: {Message}", doc.Id, ex.Message);
                throw new Exception($"Failed to index document: {ex.Message}", ex);
            }
        }

        public async Task<List<DocumentResposeDto>> SearchSimilarAsync(float[] embedding, int topK = 5)
        {
            try
            {
                var request = new QdrantSearchRequestDto
                {
                    Vector = embedding
                };
                var json = System.Text.Json.JsonSerializer.Serialize(request);

                _logger?.LogDebug("Searching similar documents in Qdrant: {Request}", json);

                var response = await _qdrantApi.SearchPointsAsync(_options.CollectionName, request);

                _logger?.LogInformation("Search completed in Qdrant. Found {Count} results.", response.Result.Count);

                return [.. response.Result.Select(r => new DocumentResposeDto
                (
                    Guid.Parse(r.Id),
                    r.Payload?.Text ?? string.Empty
                ))];
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to search similar documents in Qdrant: {Message}", ex.Message);
                throw new Exception("Failed to search similar documents. Please check the Qdrant service and your connection settings.", ex);
            }
        }
    }
}
