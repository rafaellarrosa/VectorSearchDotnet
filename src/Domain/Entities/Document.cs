namespace Domain.Entities;

public class Document
{
    public required string Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = [];
}