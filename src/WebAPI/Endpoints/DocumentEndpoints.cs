using System;
using Application.UseCases;

namespace WebAPI.Endpoints;

public static class DocumentEndpoints
{
    public static void MapDocumentEndpoints(this WebApplication app)
    {
        app.MapPost("/documents", async (string text, IndexDocumentUseCase useCase) =>
        {
            await useCase.ExecuteAsync(text);
            return Results.Ok(new { message = "Indexed successfully." });
        });

        app.MapGet("/search", async (string query, int topK, SearchSimilarDocumentsUseCase useCase) =>
        {
            var results = await useCase.ExecuteAsync(query, topK);
            return Results.Ok(results.Select(r => new { r.Id, r.Text }));
        });
    }
}
