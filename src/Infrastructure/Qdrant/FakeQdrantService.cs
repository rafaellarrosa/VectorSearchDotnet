using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Qdrant;

public class FakeQdrantService : IVectorDatabaseService
{
    private static readonly List<Document> _documents = new();

    public Task IndexDocumentAsync(Document doc)
    {
        _documents.Add(doc);
        Console.WriteLine($"[FakeQdrant] Document indexed: {doc.Id}");
        return Task.CompletedTask;
    }

    public Task<List<Document>> SearchSimilarAsync(float[] embedding, int topK = 5)
    {
        var results = _documents
            .Select(doc => new
            {
                Document = doc,
                Score = CosineSimilarity(doc.Embedding, embedding)
            })
            .OrderByDescending(x => x.Score)
            .Take(topK)
            .Select(x => x.Document)
            .ToList();

        return Task.FromResult(results);
    }

    private static float CosineSimilarity(float[] v1, float[] v2)
    {
        float dot = 0, normA = 0, normB = 0;
        for (int i = 0; i < v1.Length; i++)
        {
            dot += v1[i] * v2[i];
            normA += v1[i] * v1[i];
            normB += v2[i] * v2[i];
        }
        return dot / ((float)Math.Sqrt(normA) * (float)Math.Sqrt(normB) + 1e-8f);
    }
}