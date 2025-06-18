namespace Application.Interfaces
{
    public interface IIaService
    {
        Task<string> GenerateAnswerAsync(string question, List<string> documents);
    }
}
