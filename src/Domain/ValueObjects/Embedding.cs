using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object representing a Document embedding vector
/// Encapsulates embedding-specific business rules and operations
/// </summary>
public readonly record struct Embedding
{
    public float[] Vector { get; }

    [JsonConstructor]
    public Embedding(float[] vector)
    {
        if (vector == null || vector.Length == 0)
            throw new ArgumentException("Embedding vector cannot be null or empty", nameof(vector));

        if (vector.Any(v => float.IsNaN(v) || float.IsInfinity(v)))
            throw new ArgumentException("Embedding vector cannot contain NaN or Infinity values", nameof(vector));

        Vector = vector.ToArray(); // Create defensive copy
    }

    public static Embedding From(float[] vector) => new(vector);

    public float[] ToArray() => Vector.ToArray(); // Return defensive copy

    public int Dimensions => Vector.Length;

    /// <summary>
    /// Calculates cosine similarity between this embedding and another
    /// </summary>
    public double CosineSimilarity(Embedding other)
    {
        if (Dimensions != other.Dimensions)
            throw new ArgumentException("Embeddings must have the same dimensions for similarity calculation");

        var dotProduct = Vector.Zip(other.Vector, (a, b) => a * b).Sum();
        var magnitudeA = Math.Sqrt(Vector.Select(x => (double)x * x).Sum());
        var magnitudeB = Math.Sqrt(other.Vector.Select(x => (double)x * x).Sum());

        if (magnitudeA == 0 || magnitudeB == 0)
            return 0;

        return dotProduct / (magnitudeA * magnitudeB);
    }

    public static implicit operator float[](Embedding embedding) => embedding.ToArray();
    public static explicit operator Embedding(float[] vector) => new(vector);
}
