using System;
using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class SearchSimilarDocumentsQuery(string query) : IRequest<IEnumerable<DocumentResposeDto>?>
{
    public string SearchQuery { get; private set; } = query;
}
