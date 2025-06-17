using System.Text.Json.Serialization;

namespace Infrastructure.DTOs
{
    public class EmbeddingRequestDto
    {
        public string text { get; set; } = string.Empty;
    }
}
