namespace Infrastructure.Configurations;

public class QdrantOptions
{
    public string BaseUrl { get; set; } = "http://localhost:6333";
    public string CollectionName { get; set; } = "documents";
    public float SimilarityThreshold { get; set; } = 0.7f; // valor default

}