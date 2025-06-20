using System;
using Application.Commands;
using Application.Queries;
using Infrastructure.Seed;
using MediatR;

namespace WebAPI.Endpoints
{
    public static class DocumentEndpoints
    {
        public static void MapDocumentEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapPost("/documents", async (string title, string text, IMediator mediator) =>
            {
                await mediator.Send(new IndexDocumentCommand(title, text));
                return Results.Ok(new { message = "Indexed successfully." });
            });

            app.MapGet("/search", async (string query, int topK, IMediator mediator) =>
            {
                var result = await mediator.Send(new SearchSimilarDocumentsQuery(query)); //useCase.ExecuteAsync(query, topK);
                return Results.Ok(result);
            });

            app.MapPost("/seed", async (SeedService seedService) =>
            {
                await seedService.SeedAsync();
                return Results.Ok("Seed executado com sucesso.");
            });
        }
    }
}