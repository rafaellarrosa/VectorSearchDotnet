namespace Infrastructure.Services.TextGeneration;

public class TextGenerationOptions
{
    public string BaseUrl { get; set; } = "https://api-inference.huggingface.co";
    public string DefaultModel { get; set; } = "microsoft/DialoGPT-medium";
    public int DefaultMaxTokens { get; set; } = 100;
    public float DefaultTemperature { get; set; } = 0.7f;
}
