using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.HuggingFace;

public class HuggingFaceEmbeddingRequestDto
{
    [JsonPropertyName("inputs")]
    public string Inputs { get; set; } = string.Empty;

    [JsonPropertyName("options")]
    public HuggingFaceOptionsDto? Options { get; set; }
}

public class HuggingFaceOptionsDto
{
    [JsonPropertyName("wait_for_model")]
    public bool WaitForModel { get; set; } = true;

    [JsonPropertyName("use_cache")]
    public bool UseCache { get; set; } = true;
}
