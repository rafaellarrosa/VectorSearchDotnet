namespace Infrastructure.Qdrant;

public class QdrantOptions
{
    public string BaseUrl { get; set; } = "http://localhost:6333";
    public string CollectionName { get; set; } = "documents";
}