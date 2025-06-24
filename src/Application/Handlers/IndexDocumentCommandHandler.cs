using Application.Commands;
using Application.DTOs;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class IndexDocumentCommandHandler(
    IDocumentRetrievalService documentRetrievalService,
    ILogger<IndexDocumentCommandHandler> logger
) : IRequestHandler<IndexDocumentCommand, Unit>
{
    private readonly IDocumentRetrievalService _documentRetrievalService = documentRetrievalService ?? throw new ArgumentNullException(nameof(documentRetrievalService));
    private readonly ILogger<IndexDocumentCommandHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Unit> Handle(IndexDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Handling IndexDocumentCommand for text: {Text}", request.Text);
        
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.Text))
            throw new ArgumentException("Text cannot be null or empty.", nameof(request.Text));

        await _documentRetrievalService.IndexDocumentAsync(request.Text, request.Title);

        _logger?.LogInformation("Document indexed successfully for title: {Title}", request.Title);

        return Unit.Value;
    }
}

