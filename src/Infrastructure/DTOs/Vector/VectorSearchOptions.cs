using System;

namespace Infrastructure.DTOs.Vector;

public class VectorSearchOptions
{
    public int TopK { get; set; } = 5;
    public float DefaultSimilarityThreshold { get; set; } = 0.7f;
}
