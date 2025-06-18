using Infrastructure.DTOs.HuggingFace;
using Refit;

namespace Infrastructure.Clients;

public interface IHuggingFaceApi
{
    [Post("/models/{model}")]
    Task<float[]> GenerateEmbeddingAsync(
        string model, 
        [Body] HuggingFaceEmbeddingRequestDto request,
        [Header("Authorization")] string authorization);

    [Post("/models/{model}")]
    Task<HuggingFaceTextGenerationResponseDto[]> GenerateTextAsync(
        string model, 
        [Body] HuggingFaceTextGenerationRequestDto request,
        [Header("Authorization")] string authorization);
}
