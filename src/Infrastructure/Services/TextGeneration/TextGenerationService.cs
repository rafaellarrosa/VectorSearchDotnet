using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.TextGeneration;

public class TextGenerationService : ITextGenerationService
{
    private readonly IHuggingFaceService _huggingFaceService;
    private readonly ILogger<TextGenerationService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _defaultModel;
    private readonly int _defaultMaxTokens;
    private readonly float _defaultTemperature;

    public TextGenerationService(
        IHuggingFaceService huggingFaceService,
        ILogger<TextGenerationService> logger,
        IConfiguration configuration)
    {
        _huggingFaceService = huggingFaceService ?? throw new ArgumentNullException(nameof(huggingFaceService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        
        _defaultModel = _configuration["TextGenerationOptions:DefaultModel"] ?? "microsoft/DialoGPT-medium";
        _defaultMaxTokens = int.TryParse(_configuration["TextGenerationOptions:DefaultMaxTokens"], out var maxTokens) ? maxTokens : 100;
        _defaultTemperature = float.TryParse(_configuration["TextGenerationOptions:DefaultTemperature"], out var temperature) ? temperature : 0.7f;
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        return await GenerateTextAsync(prompt, _defaultMaxTokens, _defaultTemperature);
    }

    public async Task<string> GenerateTextAsync(string prompt, int maxTokens = 100, float temperature = 0.7f)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt, nameof(prompt));
        
        _logger?.LogDebug("Generating text for prompt: {Prompt} with maxTokens: {MaxTokens}, temperature: {Temperature}", prompt, maxTokens, temperature);

        try
        {
            var generatedText = await _huggingFaceService.GenerateTextAsync(prompt, maxTokens, temperature, _defaultModel);
            
            _logger?.LogInformation("Successfully generated text for prompt: {Prompt}", prompt);
            return generatedText;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Failed to generate text for prompt: {Prompt}", prompt);
            throw;
        }
    }
}
