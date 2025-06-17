using Application.Interfaces;
using Infrastructure.Clients;
using Infrastructure.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Infrastructure.Services.Embedding
{
    public class EmbeddingService(
        IEmbeddingApi embeddingApi,
        ILogger<EmbeddingService> logger,
        HttpClient httpClient,
        IConfiguration configuration) : IEmbeddingService
    {
        private readonly IEmbeddingApi _embeddingApi = embeddingApi;
        private readonly ILogger<EmbeddingService> _logger = logger;
        private readonly HttpClient _httpClient = httpClient;
        private readonly string _embeddingEndpointUrl = configuration["Embedding:EndpointUrl"] ?? "http://localhost:7071/api/embed";

        public async Task<float[]> GenerateEmbeddingAsync(string text)
        {
            _logger?.LogDebug("Requesting embedding for text: {Text}", text);
            try
            {
                var response = await _embeddingApi.EmbedAsync(new { text = text });

                _logger?.LogInformation("Embedding generated successfully for text: {Text}", text);

                return response.Embedding ?? [];
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to generate embedding for text: {Text}", text);

                throw;
            }
        }

        public async Task<float[]> GenerateEmbeddingWithHttpClientAsync(string text)
        {
            _logger?.LogDebug("[Experimental] Requesting embedding via HttpClient for text: {Text}", text);
            try
            {
                var payload = new { text };
                var json = System.Text.Json.JsonSerializer.Serialize(payload);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_embeddingEndpointUrl, content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                var embeddingResponse = System.Text.Json.JsonSerializer.Deserialize<EmbeddingResponseDto>(responseString);

                _logger?.LogInformation("[Experimental] Embedding generated via HttpClient for text: {Text}", text);
                return embeddingResponse?.Embedding ?? [];
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "[Experimental] Failed to generate embedding via HttpClient for text: {Text}", text);
                throw;
            }
        }
    }
}