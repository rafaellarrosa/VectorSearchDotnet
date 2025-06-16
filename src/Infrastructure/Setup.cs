using System;
using Application.Interfaces;
using Infrastructure.Clients;
using Infrastructure.Services.Embedding;
using Infrastructure.Services.Qdrant;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Refit;

namespace Infrastructure
{
    public static class Setup
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QdrantOptions>(configuration.GetSection("QdrantOptions"));

            // Registra o cliente HTTP para a API de embedding
            services.AddRefitClient<IEmbeddingApi>()
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri(configuration["EmbeddingOptions:BaseUrl"] ?? throw new ArgumentNullException("EmbeddingOptions:BaseUrl", "BaseUrl for EmbeddingOptions is not configured."));
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

            // Registra o serviço de embedding fake
            services.AddScoped<IEmbeddingService, EmbeddingService>();
        }
    }
}
