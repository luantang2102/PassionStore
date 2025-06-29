using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Infrastructure.Data;
using System;
using System.Threading.Tasks;

namespace PassionStore.Infrastructure.Repositories
{
    public class HelpfulVoteRepository : IHelpfulVoteRepository
    {
        private readonly AppDbContext _context;

        public HelpfulVoteRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HelpfulVote?> GetByUserAndRatingAsync(Guid userId, Guid ratingId)
        {
            return await _context.HelpfulVotes
                .FirstOrDefaultAsync(hv => hv.UserId == userId && hv.RatingId == ratingId);
        }

        public async Task CreateAsync(HelpfulVote helpfulVote)
        {
            await _context.HelpfulVotes.AddAsync(helpfulVote);
        }

        public async Task DeleteAsync(HelpfulVote helpfulVote)
        {
            _context.HelpfulVotes.Remove(helpfulVote);
            await Task.CompletedTask;
        }
    }
}