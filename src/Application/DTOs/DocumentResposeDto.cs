namespace Application.DTOs;

public class DocumentResposeDto(Guid id, string text)
{

    public Guid Id { get; private set; } = id;

    public string Text { get; private set; } = text;
}
