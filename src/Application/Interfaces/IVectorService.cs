using System;
using Application.DTOs;

namespace Application.Interfaces;

public interface IVectorService
{
    Task<Guid> IndexAsync(string text, Guid? externalId = null);

    Task<List<DocumentResposeDto>> SearchAsync(string query, float? thresholdOverride = null);
}
