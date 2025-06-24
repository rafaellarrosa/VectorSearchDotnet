using System;
using Application.DTOs;

namespace Application.Interfaces;

public interface IDocumentRetrievalService
{
    Task<Guid> IndexDocumentAsync(string text, string title);

    Task<List<EnrichedDocumentDto>> RetrieveDocumentsAsync(string query, float? thresholdOverride = null);
}
