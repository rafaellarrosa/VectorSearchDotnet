using Domain.Entities;
using Domain.ValueObjects;

namespace Domain.Services;

/// <summary>
/// Domain service for complex document operations
/// Contains business logic that doesn't naturally belong to a single entity
/// </summary>
public interface IDocumentDomainService
{
    /// <summary>
    /// Creates a document with embedding generation
    /// </summary>
    Task<Document> CreateDocumentWithEmbeddingAsync(string text, Func<string, Task<float[]>> generateEmbedding);

    /// <summary>
    /// Finds the most similar documents from a collection
    /// </summary>
    IEnumerable<(Document Document, double Similarity)> FindMostSimilar(
        Document sourceDocument, 
        IEnumerable<Document> candidateDocuments, 
        int topCount = 5);

    /// <summary>
    /// Determines if two documents are considered duplicates based on similarity threshold
    /// </summary>
    bool AreDuplicates(Document document1, Document document2, double threshold = 0.95);
}

public class DocumentDomainService : IDocumentDomainService
{
    public async Task<Document> CreateDocumentWithEmbeddingAsync(string text, Func<string, Task<float[]>> generateEmbedding)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be null or empty", nameof(text));

        if (generateEmbedding == null)
            throw new ArgumentNullException(nameof(generateEmbedding));

        var document = Document.Create(text);
        
        try
        {
            var embeddingVector = await generateEmbedding(text);
            document.SetEmbedding(embeddingVector);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to generate embedding for document", ex);
        }

        return document;
    }

    public IEnumerable<(Document Document, double Similarity)> FindMostSimilar(
        Document sourceDocument, 
        IEnumerable<Document> candidateDocuments, 
        int topCount = 5)
    {
        if (sourceDocument == null)
            throw new ArgumentNullException(nameof(sourceDocument));

        if (candidateDocuments == null)
            throw new ArgumentNullException(nameof(candidateDocuments));

        if (!sourceDocument.HasEmbedding)
            throw new InvalidOperationException("Source document must have an embedding");

        if (topCount <= 0)
            throw new ArgumentException("Top count must be positive", nameof(topCount));

        return candidateDocuments
            .Where(d => d.HasEmbedding && !d.Id.Equals(sourceDocument.Id))
            .Select(d => (Document: d, Similarity: sourceDocument.CalculateSimilarity(d)))
            .OrderByDescending(x => x.Similarity)
            .Take(topCount);
    }

    public bool AreDuplicates(Document document1, Document document2, double threshold = 0.95)
    {
        if (document1 == null || document2 == null)
            return false;

        if (document1.Id.Equals(document2.Id))
            return true;

        if (!document1.HasEmbedding || !document2.HasEmbedding)
            return false;

        var similarity = document1.CalculateSimilarity(document2);
        return similarity >= threshold;
    }
}
