using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

/// <summary>
/// Value Object representing a Document identifier
/// Ensures that Document IDs are always valid and properly formatted
/// </summary>
public readonly record struct DocumentId
{
    public Guid Value { get; }

    [JsonConstructor]
    public DocumentId(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("DocumentId cannot be empty", nameof(value));
        
        Value = value;
    }

    public static DocumentId New() => new(Guid.NewGuid());
    public static DocumentId From(Guid value) => new(value);
    public static DocumentId From(string value) => new(Guid.Parse(value));

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(DocumentId id) => id.Value;
    public static implicit operator string(DocumentId id) => id.Value.ToString();
    public static explicit operator DocumentId(Guid value) => new(value);
    public static explicit operator DocumentId(string value) => From(value);
}
