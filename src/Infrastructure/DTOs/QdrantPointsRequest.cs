using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Infrastructure.DTOs;

public class QdrantPointsRequestDto
{
    [JsonPropertyName("points")]
    public IEnumerable<QdrantPointDto> Points { get; set; } = [];
}
