namespace Application.Interfaces
{
    public interface IEmbeddingService
    {
        Task<float[]> GenerateEmbeddingAsync(string text);
        Task<float[]> GenerateEmbeddingWithHttpClientAsync(string text);
    }
}