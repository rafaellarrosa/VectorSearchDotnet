using System;
using Application.DTOs;
using MediatR;

namespace Application.Queries;

public class SearchSimilarDocumentsQuery(string query) : IRequest<string>
{
    public string SearchQuery { get; private set; } = query;
}
