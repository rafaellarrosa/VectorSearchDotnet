namespace Application.Interfaces;

public interface IHuggingFaceService
{
    Task<float[]> GenerateEmbeddingAsync(string text, string? model = null);
    Task<string> GenerateTextAsync(string prompt, string? model = null);
    Task<string> GenerateTextAsync(string prompt, int maxTokens = 100, float temperature = 0.7f, string? model = null);
}
