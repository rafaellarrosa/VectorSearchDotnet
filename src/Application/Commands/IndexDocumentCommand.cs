using System;
using MediatR;

namespace Application.Commands;

public class IndexDocumentCommand(string title, string text) : IRequest<Unit>
{
    public string Title { get; private set; } = title;

    public string Text { get; private set; } = text;
}
