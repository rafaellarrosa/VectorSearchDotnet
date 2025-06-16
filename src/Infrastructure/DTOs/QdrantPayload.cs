using System.Text.Json.Serialization;

namespace Infrastructure.DTOs;

public class QdrantPayloadDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
