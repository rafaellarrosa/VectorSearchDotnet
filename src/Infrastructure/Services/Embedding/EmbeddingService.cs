using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.Tokenizers;
using System.Text.RegularExpressions;
using Microsoft.ML.OnnxRuntime.Tensors;
using Infrastructure.Interfaces;

namespace Infrastructure.Services.Embedding;

public partial class EmbeddingService : IEmbeddingService, IDisposable
{
    private readonly ILogger<EmbeddingService> _logger;
    private readonly InferenceSession _inferenceSession;
    private readonly BertTokenizer _tokenizer;
    private readonly Regex _cleanRegex;
    private readonly int _embeddingSize = 384;

    public EmbeddingService(ILogger<EmbeddingService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        string modelPath = configuration["EmbeddingOptions:ModelPath"] ?? throw new ArgumentNullException("Model path is not configured.");

        var vocabPath = Path.Combine(modelPath, "vocab.txt");
        var onnxPath = Path.Combine(modelPath, "onnx", "model.onnx");

        _inferenceSession = new InferenceSession(onnxPath, new SessionOptions());
        _tokenizer = BertTokenizer.Create(vocabPath);
        _cleanRegex = MyRegex();
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text)
    {
        _logger.LogDebug("Generating embedding for text: {Text}", text);
        try
        {
            var cleanedText = _cleanRegex.Replace(text, string.Empty);

            var inputIds = _tokenizer.EncodeToIds(cleanedText);
            int sequenceLength = inputIds.Count;

            var attentionMask = Enumerable.Repeat(1L, sequenceLength).ToArray();
            var tokenTypeIds = new long[sequenceLength];

            using var inputIdsOrt = OrtValue.CreateTensorValueFromMemory(inputIds.Select(i => (long)i).ToArray(), new long[] { 1, sequenceLength });
            using var attentionMaskOrt = OrtValue.CreateTensorValueFromMemory(attentionMask, new long[] { 1, sequenceLength });
            using var tokenTypeIdsOrt = OrtValue.CreateTensorValueFromMemory(tokenTypeIds, new long[] { 1, sequenceLength });

            var inputNames = new[] { "input_ids", "attention_mask", "token_type_ids" };
            var inputs = new OrtValue[] { inputIdsOrt, attentionMaskOrt, tokenTypeIdsOrt };

            using var output = OrtValue.CreateAllocatedTensorValue(OrtAllocator.DefaultInstance, TensorElementType.Float, new long[] { 1, sequenceLength, _embeddingSize });
            await _inferenceSession.RunAsync(new RunOptions(), inputNames, inputs, _inferenceSession.OutputNames, new[] { output });

            return ProcessOutput(output, attentionMask, sequenceLength);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate embedding");
            throw;
        }
    }

    private float[] ProcessOutput(OrtValue output, long[] attentionMask, int sequenceLength)
    {
        var tensorSpan = output.GetTensorDataAsSpan<float>();
        var tensorData = tensorSpan.ToArray();
        var pooled = MeanPooling(tensorData, attentionMask, sequenceLength);
        var normalized = Normalize(pooled);
        return normalized;
    }


    private float[] MeanPooling(float[] embeddings, long[] attentionMask, int sequenceLength)
    {
        var sum = new float[_embeddingSize];
        int validTokens = 0;

        for (int tokenIdx = 0; tokenIdx < sequenceLength; tokenIdx++)
        {
            if (attentionMask[tokenIdx] == 0) continue;
            for (int i = 0; i < _embeddingSize; i++)
            {
                sum[i] += embeddings[tokenIdx * _embeddingSize + i];
            }
            validTokens++;
        }

        if (validTokens > 0)
        {
            for (int i = 0; i < _embeddingSize; i++)
                sum[i] /= validTokens;
        }

        return sum;
    }

    private float[] Normalize(float[] vector)
    {
        double sumSquares = vector.Sum(v => v * v);
        double norm = Math.Sqrt(Math.Max(sumSquares, 1e-9));
        return vector.Select(v => (float)(v / norm)).ToArray();
    }

    public void Dispose()
    {
        _inferenceSession.Dispose();
    }

    [GeneratedRegex(@"[\u0000-\u001F\u007F-\uFFFF]", RegexOptions.Compiled)]
    private static partial Regex MyRegex();
}
