namespace Application.Interfaces;

public interface ITextGenerationService
{
    Task<string> GenerateTextAsync(string prompt);
    Task<string> GenerateTextAsync(string prompt, int maxTokens = 100, float temperature = 0.7f);
}
