using Application.Interfaces;
using Infrastructure.DTOs;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Infrastructure.DTOs.Embedding;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.Embedding
{
    public class EmbeddingService(HttpClient httpClient, ILogger<EmbeddingService> logger, IConfiguration configuration) : IEmbeddingService
    {
        private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        private readonly ILogger<EmbeddingService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            _logger?.LogDebug("Requesting embedding for text: {Text}", text);
            try
            {
                var payload = new { text };
                var json = JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(new Uri($"{configuration["EmbeddingOptions:BaseUrl"]}/embed"), content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var embeddingResponse = JsonSerializer.Deserialize<EmbeddingResponseDto>(responseString);

                _logger?.LogInformation("Embedding generated successfully for text: {Text}", text);
                return embeddingResponse?.Embedding ?? Array.Empty<float>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to generate embedding for text: {Text}", text);
                throw;
            }
        }
    }
}