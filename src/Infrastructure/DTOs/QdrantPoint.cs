using System.Text.Json.Serialization;

namespace Infrastructure.DTOs;

public class QdrantPointDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("vector")]
    public float[] Vector { get; set; } = [];

    [JsonPropertyName("payload")]
    public QdrantPayloadDto Payload { get; set; } = new();
}
