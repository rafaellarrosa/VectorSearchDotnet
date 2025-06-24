namespace Application.DTOs;

public class DocumentResposeDto(Guid id, string text, float score)
{
    public Guid Id { get; set; } = id;
    public string Text { get; set; } = text;
    public float Score { get; set; } = score;
}
