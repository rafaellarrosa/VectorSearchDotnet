using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Infrastructure.Clients;
using Infrastructure.Services.Embedding;
using Infrastructure.Services.HuggingFace;
using Infrastructure.Services.Qdrant;
using Infrastructure.Services.TextGeneration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure
{
    public static class Setup
    {        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Options
            services.Configure<QdrantOptions>(configuration.GetSection("QdrantOptions"));
            services.Configure<HuggingFaceOptions>(configuration.GetSection("HuggingFace"));
            services.Configure<TextGenerationOptions>(configuration.GetSection("TextGenerationOptions"));

            // Registra o cliente HTTP para a API de embedding
            services.AddRefitClient<IEmbeddingApi>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            })
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(configuration["EmbeddingOptions:BaseUrl"] ?? throw new ArgumentNullException("EmbeddingOptions:BaseUrl", "BaseUrl for EmbeddingOptions is not configured."));
            });

            // Registra o cliente HTTP para a API do HuggingFace
            services.AddRefitClient<IHuggingFaceApi>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                })
            })
            .ConfigureHttpClient(client =>
            {
                var baseUrl = configuration["HuggingFace:BaseUrl"] ?? "https://api-inference.huggingface.co";
                client.BaseAddress = new Uri(baseUrl);
                
                var timeoutSeconds = int.TryParse(configuration["HuggingFace:TimeoutSeconds"], out var timeout) ? timeout : 30;
                client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            });

            // Registra o cliente HTTP para a API do Qdrant usando Refit
            services.AddRefitClient<IQdrantApi>()
                .ConfigureHttpClient((provider, client) =>
                {
                    var options = provider.GetRequiredService<IOptions<QdrantOptions>>().Value;
                    client.BaseAddress = new Uri(options.BaseUrl);
                });

            // Registra o serviço real do Qdrant com logger
            services.AddScoped(provider =>
            {
                var qdrantApi = provider.GetRequiredService<IQdrantApi>();
                var options = provider.GetRequiredService<IOptions<QdrantOptions>>();
                var logger = provider.GetRequiredService<Microsoft.Extensions.Logging.ILogger<QdrantService>>();

                return new QdrantService(qdrantApi, options, logger);
            });

            services.AddScoped<IVectorDatabaseService>(provider => provider.GetRequiredService<QdrantService>());

            // Registra os serviços de IA
            services.AddScoped<IEmbeddingService, EmbeddingService>();
            services.AddScoped<IHuggingFaceService, HuggingFaceService>();
            services.AddScoped<ITextGenerationService, TextGenerationService>();
        }
    }
}