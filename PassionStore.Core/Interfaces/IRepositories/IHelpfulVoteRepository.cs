using PassionStore.Core.Entities;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IHelpfulVoteRepository
    {
        Task<HelpfulVote?> GetByUserAndRatingAsync(Guid userId, Guid ratingId);
        Task CreateAsync(HelpfulVote helpfulVote);
        Task DeleteAsync(HelpfulVote helpfulVote);
    }
}