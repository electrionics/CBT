using CBT.Domain.Entities;

namespace CBT.Logic.Contracts
{
    public interface IAutomaticThoughtsService
    {
        Task<List<AutomaticThought>> GetAllThoughts(string? userId = null);
        Task<int> AddThought(string thought);
        Task<int> AddThoughtFull(AutomaticThought data);
        Task<AutomaticThought?> GetThought(int id);
        Task EditThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId);
        Task DeleteThought(int id);
        Task SendThoughtToPsychologist(int id);
        Task<List<ThoughtPsychologistReview>> GetPsychologistReviews(int thoughtId);
        Task<List<AutomaticThought>> GetAllAutomaticThoughts(string? userId = null);
        Task<int> AddAutomaticThoughtFull(AutomaticThought data, string? userId);
        Task<AutomaticThought> GetAutomaticThought(int id);
        Task EditAutomaticThoughtFull(Action<AutomaticThought, int> convertBack, int thoughtId, string? userId);
    }
}
