using System;
using Application.Commands;
using Application.Interfaces;
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
        });        app.MapGet("/search", async (string query, int topK, IMediator mediator) =>
        {
            var results = await mediator.Send(new SearchSimilarDocumentsQuery(query)); //useCase.ExecuteAsync(query, topK);
            return Results.Ok(results?.Select(r => new { r.Id, r.Text }).ToArray() ?? Array.Empty<object>());
        });

        // Text Generation endpoints
        app.MapPost("/generate-text", async (string prompt, ITextGenerationService textGenerationService) =>
        {
            var generatedText = await textGenerationService.GenerateTextAsync(prompt);
            return Results.Ok(new { prompt, generatedText });
        });

        app.MapPost("/generate-text/advanced", async (
            string prompt, 
            int maxTokens, 
            float temperature, 
            ITextGenerationService textGenerationService) =>
        {
            var generatedText = await textGenerationService.GenerateTextAsync(prompt, maxTokens, temperature);
            return Results.Ok(new { prompt, generatedText, maxTokens, temperature });
        });

        // HuggingFace direct endpoints
        app.MapPost("/huggingface/embedding", async (string text, IHuggingFaceService huggingFaceService) =>
        {
            var embedding = await huggingFaceService.GenerateEmbeddingAsync(text);
            return Results.Ok(new { text, embedding = embedding.Take(10).ToArray(), fullSize = embedding.Length });
        });        app.MapPost("/huggingface/generate", async (string prompt, IHuggingFaceService huggingFaceService) =>
        {
            var generatedText = await huggingFaceService.GenerateTextAsync(prompt, model: null);
            return Results.Ok(new { prompt, generatedText });
        });
    }
}
