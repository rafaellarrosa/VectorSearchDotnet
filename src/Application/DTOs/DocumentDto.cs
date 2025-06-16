using System;

namespace Application.DTOs;

public class DocumentDto
{
    public DocumentDto(string text, float[] embedding)
    {
        Id = Guid.NewGuid();
        Text = text;
        Embedding = embedding;
    }

    public DocumentDto(Guid id, string text, float[] embedding)
    {
        Id = id;
        Text = text;
        Embedding = embedding;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();

    public string Text { get; set; }
    public float[] Embedding { get; set; }
}
