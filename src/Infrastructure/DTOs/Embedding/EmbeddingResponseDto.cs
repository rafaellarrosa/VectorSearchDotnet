using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.Embedding
{
    public class EmbeddingResponseDto
    {
        [JsonPropertyName("embedding")]
        public float[]? Embedding { get; set; }
    }
}
