using System;
using Application.Interfaces;
using Domain.Entities;

namespace Application.UseCases;

public class IndexDocumentUseCase
{
    private readonly IEmbeddingService _embeddingService;
    private readonly IVectorDatabaseService _vectorDbService;

    public IndexDocumentUseCase(IEmbeddingService embeddingService, IVectorDatabaseService vectorDbService)
    {
        _embeddingService = embeddingService;
        _vectorDbService = vectorDbService;
    }

    public async Task ExecuteAsync(string text)
    {
        var embedding = await _embeddingService.GenerateEmbeddingAsync(text);

        var document = new Document
        {
            Id = Guid.NewGuid(),
            Text = text,
            Embedding = embedding
        };

        await _vectorDbService.IndexDocumentAsync(document);
    }
}