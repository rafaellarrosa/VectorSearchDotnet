using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.Qdrant;

public class QdrantPayloadDto
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = string.Empty;
}
