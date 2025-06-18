using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.Qdrant;

public class QdrantPointsRequestDto
{
    [JsonPropertyName("points")]
    public IEnumerable<QdrantPointDto> Points { get; set; } = [];
}
