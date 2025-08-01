using System;
using Application.DTOs;
using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IVectorDatabaseService
{
    Task<Guid> IndexDocumentAsync(DocumentDto doc);

    Task<List<DocumentResposeDto>> SearchSimilarAsync(float[] embedding, int topK = 5);
}
