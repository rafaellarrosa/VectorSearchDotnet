using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object representing Document text content
/// Encapsulates business rules for text validation and formatting
/// </summary>
public readonly record struct DocumentText
{
    private const int MaxLength = 10000;
    
    public string Value { get; }

    [JsonConstructor]
    public DocumentText(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Document text cannot be null or empty", nameof(value));
        
        if (value.Length > MaxLength)
            throw new ArgumentException($"Document text cannot exceed {MaxLength} characters", nameof(value));

        Value = value.Trim();
    }

    public static DocumentText From(string value) => new(value);

    public int Length => Value.Length;
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    public override string ToString() => Value;

    public static implicit operator string(DocumentText text) => text.Value;
    public static explicit operator DocumentText(string value) => new(value);
}
