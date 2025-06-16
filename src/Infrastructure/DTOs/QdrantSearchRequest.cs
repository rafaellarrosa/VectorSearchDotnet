using System.Text.Json.Serialization;

namespace Infrastructure.DTOs;

public class QdrantSearchRequestDto
{    public required float[] Vector { get; set; }

    [JsonPropertyName("top")]
    public int Top { get; set; } = 5;

    public Dictionary<string, object>? Filter { get; set; }

    [JsonPropertyName("with_payload")]
    public bool WithPayload { get; set; } = true;

    [JsonPropertyName("with_vector")]
    public bool WithVector { get; set; } = false;
}
