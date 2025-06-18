using Infrastructure.DTOs;
using Refit;
using System.Threading.Tasks;
using Infrastructure.DTOs.Embedding;

namespace Infrastructure.Clients
{
    public interface IEmbeddingApi
    {
        [Post("/embed")]
        Task<EmbeddingResponseDto> EmbedAsync([Body] object request);
    }
}
