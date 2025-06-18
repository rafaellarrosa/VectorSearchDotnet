using System.Text.Json.Serialization;

namespace Infrastructure.DTOs.HuggingFace;

public class HuggingFaceTextGenerationRequestDto
{
    [JsonPropertyName("inputs")]
    public string Inputs { get; set; } = string.Empty;

    [JsonPropertyName("parameters")]
    public HuggingFaceTextParametersDto? Parameters { get; set; }

    [JsonPropertyName("options")]
    public HuggingFaceOptionsDto? Options { get; set; }
}

public class HuggingFaceTextParametersDto
{
    [JsonPropertyName("max_length")]
    public int? MaxLength { get; set; }

    [JsonPropertyName("max_new_tokens")]
    public int? MaxNewTokens { get; set; }

    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public float? TopP { get; set; }

    [JsonPropertyName("top_k")]
    public int? TopK { get; set; }

    [JsonPropertyName("do_sample")]
    public bool? DoSample { get; set; }

    [JsonPropertyName("return_full_text")]
    public bool? ReturnFullText { get; set; } = false;
}
