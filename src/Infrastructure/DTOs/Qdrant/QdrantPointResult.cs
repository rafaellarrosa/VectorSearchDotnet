namespace Infrastructure.DTOs.Qdrant;

public partial class QdrantService
{
    private class QdrantPointResultDto
    {
        public string id { get; set; } = string.Empty;
        public float[] vector { get; set; } = [];
        public QdrantPayloadDto? payload { get; set; }
    }
}
