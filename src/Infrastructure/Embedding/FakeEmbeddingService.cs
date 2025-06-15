using System;
using Application.Interfaces;

namespace Infrastructure.Embedding
{
    public class FakeEmbeddingService : IEmbeddingService
    {
        public Task<float[]> GenerateEmbeddingAsync(string text)
        {
            // Simula um embedding de 1536 posições
            var rnd = new Random();
            return Task.FromResult(Enumerable.Range(0, 1536).Select(_ => (float)rnd.NextDouble()).ToArray());
        }
    }
}