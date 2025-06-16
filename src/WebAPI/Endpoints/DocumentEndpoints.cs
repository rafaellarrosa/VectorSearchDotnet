using System;
using Application.Commands;
using Application.Queries;
using MediatR;

namespace WebAPI.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/documents", async (string text, IMediator mediator) =>
        {
            await mediator.Send(new IndexDocumentCommand(text));
            return Results.Ok(new { message = "Indexed successfully." });
        });

        app.MapGet("/search", async (string query, int topK, IMediator mediator) =>
        {
            var results = await mediator.Send(new SearchSimilarDocumentsQuery(query)); //useCase.ExecuteAsync(query, topK);
            return Results.Ok(results.Select(r => new { r.Id, r.Text }));
        });
    }
}
