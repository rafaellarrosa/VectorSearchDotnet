using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.HuggingFace;

public class HuggingFaceTextGenerationResponseDto
{
    [JsonPropertyName("generated_text")]
    public string GeneratedText { get; set; } = string.Empty;
}
