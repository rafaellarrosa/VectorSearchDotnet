using Application.Interfaces;
using Application.UseCases;
using Infrastructure.Embedding;
using Infrastructure.Qdrant;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vector Search API",
        Version = "v1"
    });
});

builder.Services.AddScoped<IEmbeddingService, FakeEmbeddingService>();
builder.Services.AddScoped<IVectorDatabaseService, FakeQdrantService>();
builder.Services.AddScoped<IndexDocumentUseCase>();
builder.Services.AddScoped<SearchSimilarDocumentsUseCase>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Vector Search API V1");
        options.RoutePrefix = string.Empty; // Swagger na raiz
    });
}

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

app.Run();