using Application.Interfaces;
using Infrastructure.Clients;
using Infrastructure.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Infrastructure.Services.Embedding
{
    public class EmbeddingService(IEmbeddingApi embeddingApi, ILogger<EmbeddingService> logger) : IEmbeddingService
    {
        private readonly IEmbeddingApi _embeddingApi = embeddingApi;
        private readonly ILogger<EmbeddingService> _logger = logger;

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            _logger?.LogDebug("Requesting embedding for text: {Text}", text);
            try
            {
                var response = await _embeddingApi.EmbedAsync(new EmbeddingRequestDto { Text = text });

                _logger?.LogInformation("Embedding generated successfully for text: {Text}", text);

                return response.Embedding ?? [];
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to generate embedding for text: {Text}", text);

                throw;
            }
        }
    }
}