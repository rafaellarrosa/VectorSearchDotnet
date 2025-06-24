using System;

namespace Application.DTOs;

public class EnrichedDocumentDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public float Score { get; set; }
}
