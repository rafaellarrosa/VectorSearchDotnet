using Infrastructure.DTOs;
using Infrastructure.DTOs.Qdrant;
using Refit;

namespace Infrastructure.Clients;

public interface IQdrantApi
{
    [Put("/collections/{collectionName}/points?wait=true")]
    Task<HttpResponseMessage> AddPointsAsync(string collectionName, [Body] QdrantPointsRequestDto request);

    [Post("/collections/{collectionName}/points/search")]
    Task<QdrantSearchResponseDto> SearchPointsAsync(string collectionName, [Body] QdrantSearchRequestDto request);

    [Put("/collections/{collectionName}")]
    Task CreateCollectionAsync(string collectionName, [Body] QdrantCreateCollectionRequestDto request);
}
