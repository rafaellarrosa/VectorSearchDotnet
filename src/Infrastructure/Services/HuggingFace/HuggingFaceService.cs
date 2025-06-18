using System.Text;
using System.Text.Json;
using Application.Interfaces;
using Infrastructure.Configurations;
using Infrastructure.DTOs.HuggingFace;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.HuggingFace
{
    public class HuggingFaceService : IIaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HuggingFaceService> _logger;
        private readonly HuggingFaceOptions _options;

        public HuggingFaceService(
            HttpClient httpClient,
            ILogger<HuggingFaceService> logger,
            IOptions<HuggingFaceOptions> options)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;

            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);

            if (!string.IsNullOrEmpty(_options.ApiKey))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
            }
        }

        public async Task<string> GenerateAnswerAsync(string question, List<string> documents)
        {
            if (string.IsNullOrWhiteSpace(question))
            {
                throw new ArgumentException("Question cannot be null or empty", nameof(question));
            }

            if (documents == null || !documents.Any())
            {
                throw new ArgumentException("Documents list cannot be null or empty", nameof(documents));
            }

            _logger.LogDebug("Generating answer for question: {Question} with {DocumentCount} documents",
                question, documents.Count);

            try
            {
                // Montar o prompt
                var context = string.Join("\n\n", documents.Where(d => !string.IsNullOrWhiteSpace(d)));
                var prompt = $@"Responda com base apenas no contexto abaixo.

                                Contexto:
                                {context}

                                Pergunta:
                                {question}

                                Resposta:";

                var url = $"{_options.BaseUrl}/models/{_options.TextGenerationModel}";
                var request = new HuggingFaceTextGenerationRequestDto { Inputs = prompt };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogDebug("Sending text generation request to Hugging Face API: {Url}", url);

                var response = await _httpClient.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Hugging Face API returned error {StatusCode}: {Error}", response.StatusCode, errorContent);
                    throw new HttpRequestException($"Hugging Face API error: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                // A resposta é um array de objetos com generated_text
                var responseArray = JsonSerializer.Deserialize<HuggingFaceTextGenerationResponseDto[]>(responseContent);

                if (responseArray == null || responseArray.Length == 0)
                {
                    _logger.LogError("Invalid text generation response from Hugging Face API");
                    throw new InvalidOperationException("Invalid text generation response from Hugging Face API");
                }

                var generatedText = responseArray[0].GeneratedText;

                // Extrair a resposta após "Resposta:"
                var answerMarker = "Resposta:";
                var answerIndex = generatedText.LastIndexOf(answerMarker, StringComparison.OrdinalIgnoreCase);

                string answer;
                if (answerIndex >= 0)
                {
                    answer = generatedText.Substring(answerIndex + answerMarker.Length).Trim();
                }
                else
                {
                    // Se não encontrar o marcador, usar o texto completo como fallback
                    answer = generatedText.Trim();
                }

                _logger.LogInformation("Successfully generated answer for question: {Question}", question);

                return answer;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while generating answer for question: {Question}", question);
                throw;
            }
            catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
            {
                _logger.LogError(ex, "Timeout while generating answer for question: {Question}", question);
                throw new TimeoutException("Request to Hugging Face API timed out", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error while generating answer for question: {Question}", question);
                throw new InvalidOperationException("Failed to parse Hugging Face API response", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating answer for question: {Question}", question);
                throw;
            }
        }
    }
}
