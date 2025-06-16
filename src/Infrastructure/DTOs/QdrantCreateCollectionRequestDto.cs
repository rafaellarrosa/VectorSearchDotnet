namespace Infrastructure.DTOs;

public class QdrantCreateCollectionRequestDto
{
    public VectorsConfig vectors { get; set; } = new();
    public string? distance { get; set; } = "Cosine";
}

public class VectorsConfig
{
    public int size { get; set; }
    public string? distance { get; set; } = "Cosine";
    public string? type { get; set; } = "float";
}
