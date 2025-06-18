using System;
using Application.DTOs;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IVectorDatabaseService
    {
        Task IndexDocumentAsync(DocumentDto doc);

        Task<List<DocumentResposeDto>> SearchSimilarAsync(float[] embedding, int topK = 5);
    }
}
