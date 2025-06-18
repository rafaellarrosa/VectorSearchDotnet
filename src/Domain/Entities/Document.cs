using Domain.ValueObjects;

namespace Domain.Entities;

/// <summary>
/// Rich domain entity representing a Document with embedded vector
/// Follows DDD principles with proper encapsulation and behavior
/// </summary>
public class Document
{
    public DocumentId Id { get; private set; }
    public DocumentText Text { get; private set; }
    public Embedding? VectorEmbedding { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // Private constructor for EF Core or serialization
    private Document() { }

    // Private constructor for internal use
    private Document(DocumentId id, DocumentText text, DateTime createdAt)
    {
        Id = id;
        Text = text;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Factory method for creating new documents
    /// </summary>
    public static Document Create(string text)
    {
        var documentText = DocumentText.From(text);
        return new Document(DocumentId.New(), documentText, DateTime.UtcNow);
    }

    /// <summary>
    /// Factory method for reconstructing documents from persistence
    /// </summary>
    public static Document Reconstruct(
        DocumentId id, 
        DocumentText text, 
        Embedding? embedding, 
        DateTime createdAt, 
        DateTime? updatedAt = null)
    {
        var document = new Document(id, text, createdAt)
        {
            VectorEmbedding = embedding,
            UpdatedAt = updatedAt
        };
        
        return document;
    }

    /// <summary>
    /// Sets the embedding for this document
    /// </summary>
    public void SetEmbedding(float[] embeddingVector)
    {
        if (embeddingVector == null || embeddingVector.Length == 0)
            throw new ArgumentException("Embedding vector cannot be null or empty", nameof(embeddingVector));

        VectorEmbedding = Embedding.From(embeddingVector);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the document text and clears the embedding
    /// </summary>
    public void UpdateText(string newText)
    {
        if (string.IsNullOrWhiteSpace(newText))
            throw new ArgumentException("Text cannot be null or empty", nameof(newText));

        Text = DocumentText.From(newText);
        VectorEmbedding = null; // Clear embedding when text changes
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates similarity with another document
    /// </summary>
    public double CalculateSimilarity(Document other)
    {
        if (other == null)
            throw new ArgumentNullException(nameof(other));

        if (!HasEmbedding || !other.HasEmbedding)
            throw new InvalidOperationException("Both documents must have embeddings to calculate similarity");

        return VectorEmbedding!.Value.CosineSimilarity(other.VectorEmbedding!.Value);
    }

    /// <summary>
    /// Indicates whether this document has an embedding
    /// </summary>
    public bool HasEmbedding => VectorEmbedding.HasValue;

    /// <summary>
    /// Indicates whether this document needs an embedding update
    /// </summary>
    public bool NeedsEmbeddingUpdate => !HasEmbedding || (UpdatedAt.HasValue && VectorEmbedding == null);

    public override bool Equals(object? obj)
    {
        return obj is Document document && Id.Equals(document.Id);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}