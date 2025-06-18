using System;

using Domain.Entities;
using Domain.ValueObjects;

namespace Application.DTOs;

/// <summary>
/// Data Transfer Object for Document - only for transport, no business logic
/// </summary>
public class DocumentDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public float[] Embedding { get; set; } = Array.Empty<float>();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Parameterless constructor for serialization
    public DocumentDto() { }

    // Constructor for explicit creation
    public DocumentDto(string id, string text, float[] embedding, DateTime createdAt, DateTime? updatedAt = null)
    {
        Id = id;
        Text = text;
        Embedding = embedding;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>
    /// Factory method to create DTO from Domain entity
    /// </summary>
    public static DocumentDto FromEntity(Document document)
    {
        if (document == null)
            throw new ArgumentNullException(nameof(document));

        return new DocumentDto(
            document.Id,
            document.Text,
            document.VectorEmbedding?.ToArray() ?? Array.Empty<float>(),
            document.CreatedAt,
            document.UpdatedAt
        );
    }    /// <summary>
    /// Factory method to convert DTO to Domain entity (for reconstruction)
    /// </summary>
    public Document ToEntity()
    {
        var documentId = DocumentId.From(Id);
        var documentText = DocumentText.From(Text);
        Domain.ValueObjects.Embedding? embedding = null;
        
        if (Embedding?.Length > 0)
        {
            embedding = Domain.ValueObjects.Embedding.From(Embedding);
        }

        return Document.Reconstruct(documentId, documentText, embedding, CreatedAt, UpdatedAt);
    }
}
