using System;
using Application.Commands;
using Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Handlers;

public class CreateGraphNodeHandler(IGraphDatabaseService graphDatabaseService, ILogger<CreateGraphNodeHandler> logger) : IRequestHandler<CreateGraphNodeCommand>
{
    public async Task<Unit> Handle(CreateGraphNodeCommand request, CancellationToken cancellationToken)
    {
        await graphDatabaseService.CreateNodeAsync(request.Label, request.Properties);
        logger.LogInformation("Graph node {Label} created successfully.", request.Label);
        
        return Unit.Value;
    }
}

