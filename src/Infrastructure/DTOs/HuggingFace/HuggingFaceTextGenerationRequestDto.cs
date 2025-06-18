using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.HuggingFace;

public class HuggingFaceTextGenerationRequestDto
{
    [JsonPropertyName("inputs")]
    public string Inputs { get; set; } = string.Empty;
}
