using Application.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.DTOs.Vector;
using Infrastructure.Interfaces;
using Infrastructure.Seed;
using Infrastructure.Services.Document;
using Infrastructure.Services.Embedding;
using Infrastructure.Services.Graph;
using Infrastructure.Services.HuggingFace;
using Infrastructure.Services.Qdrant;
using Infrastructure.Services.Vector;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class Setup
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<QdrantOptions>(configuration.GetSection("QdrantOptions"));
        services.Configure<HuggingFaceOptions>(configuration.GetSection("HuggingFace"));
        services.Configure<VectorSearchOptions>(configuration.GetSection("VectorSearchOptions"));

        services.AddScoped<IVectorService, VectorService>();
        services.AddScoped<IDocumentRetrievalService, DocumentRetrievalService>();

        services.AddHttpClient<QdrantService>();
        services.AddScoped<IVectorDatabaseService, QdrantService>();

        services.AddHttpClient<EmbeddingService>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();

        services.AddHttpClient<IIaService, HuggingFaceService>();
        services.AddScoped<IIaService, HuggingFaceService>();

        services.AddScoped<IGraphDatabaseService, Neo4JGraphDatabaseService>();
        services.AddScoped<SeedService>();
    }
}