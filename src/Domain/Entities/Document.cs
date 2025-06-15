namespace Domain.Entities;

public class Document
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
}