namespace Infrastructure.Services.HuggingFace;

public class HuggingFaceOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api-inference.huggingface.co";
    public string EmbeddingModel { get; set; } = "sentence-transformers/all-MiniLM-L6-v2";
    public string TextGenerationModel { get; set; } = "microsoft/DialoGPT-medium";
    public int TimeoutSeconds { get; set; } = 30;
}
