using System;
using Domain.Entities;

namespace Application.Interfaces;

public interface IVectorDatabaseService
{
    Task IndexDocumentAsync(Document doc);

    Task<List<Document>> SearchSimilarAsync(float[] embedding, int topK = 5);
}
