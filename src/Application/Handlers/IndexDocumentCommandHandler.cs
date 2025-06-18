using Application.Commands;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class IndexDocumentCommandHandler(IVectorDatabaseService qdrantService, IEmbeddingService embeddingService, ILogger<IndexDocumentCommandHandler> logger) : IRequestHandler<IndexDocumentCommand, Unit>
{
    private readonly IVectorDatabaseService _qdrantService = qdrantService ?? throw new ArgumentNullException(nameof(qdrantService));
    private readonly IEmbeddingService _embeddingService = embeddingService ?? throw new ArgumentNullException(nameof(embeddingService));
    private readonly ILogger<IndexDocumentCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Unit> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Handling IndexDocumentCommand for text: {Text}", request.Text);

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            _logger?.LogWarning("Text is null or empty in IndexDocumentCommand.");

            throw new ArgumentException("Text cannot be null or empty.", nameof(request.Text));
        }

        var embedding = await _embeddingService.GenerateEmbeddingAsync(request.Text);

        if (embedding == null || embedding.Length == 0)
        {
            _logger?.LogError("Failed to generate embedding for text: {Text}", request.Text);

            throw new InvalidOperationException("Failed to generate embedding.");
        }

        await _qdrantService.IndexDocumentAsync(new DocumentDto(request.Text, embedding));

        _logger?.LogInformation("Document indexed successfully for text: {Text}", request.Text);

        return Unit.Value;
    }
}
