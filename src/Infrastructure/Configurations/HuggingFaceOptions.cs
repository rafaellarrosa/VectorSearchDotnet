namespace Infrastructure.Configurations;

public class HuggingFaceOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string EmbeddingModel { get; set; } = string.Empty;
    public string TextGenerationModel { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}
