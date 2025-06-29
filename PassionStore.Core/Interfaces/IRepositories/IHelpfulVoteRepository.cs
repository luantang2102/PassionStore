using PassionStore.Core.Entities;
using System;
using System.Threading.Tasks;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IHelpfulVoteRepository
    {
        Task<HelpfulVote?> GetByUserAndRatingAsync(Guid userId, Guid ratingId);
        Task CreateAsync(HelpfulVote helpfulVote);
        Task DeleteAsync(HelpfulVote helpfulVote);
    }
}