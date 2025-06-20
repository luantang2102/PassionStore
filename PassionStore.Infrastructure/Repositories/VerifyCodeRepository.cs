using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Interfaces.IRepositories;
using PassionStore.Core.Models;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Infrastructure.Repositories
{
    public class VerifyCodeRepository : IVerifyCodeRepository
    {
        private readonly AppDbContext _context;

        public VerifyCodeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(VerifyCode verifyCode)
        {
            await _context.VerifyCodes.AddAsync(verifyCode);
        }

        public async Task<VerifyCode?> GetByUserIdAndCodeAsync(Guid userId, string code)
        {
            return await _context.VerifyCodes
                .FirstOrDefaultAsync(vc => vc.UserId == userId && vc.Code == code);
        }

        public async Task UpdateAsync(VerifyCode verifyCode)
        {
            _context.VerifyCodes.Update(verifyCode);
            await Task.CompletedTask;
        }

        public async Task DeleteAsync(VerifyCode verifyCode)
        {
            _context.VerifyCodes.Remove(verifyCode);
            await Task.CompletedTask;
        }

        public async Task DeleteAllByUserIdAsync(Guid userId)
        {
            var codes = await _context.VerifyCodes
                .Where(vc => vc.UserId == userId)
                .ToListAsync();
            if (codes.Any())
            {
                _context.VerifyCodes.RemoveRange(codes);
            }
            await Task.CompletedTask;
        }
    }
}