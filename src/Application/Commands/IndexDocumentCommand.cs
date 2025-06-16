using System;
using MediatR;

namespace Application.Commands;

public class IndexDocumentCommand(string text) : IRequest<Unit>
{
    public string Text { get; private set; } = text;
}
