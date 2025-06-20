using System;
using MediatR;

namespace Application.Commands;

public class CreateGraphNodeCommand : IRequest
{
    public string Label { get; set; }
    public Dictionary<string, object> Properties { get; set; }
}