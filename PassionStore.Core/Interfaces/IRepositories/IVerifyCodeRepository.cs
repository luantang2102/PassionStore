using PassionStore.Core.Models;

namespace PassionStore.Core.Interfaces.IRepositories
{
    public interface IVerifyCodeRepository
    {
        Task CreateAsync(VerifyCode verifyCode);
        Task<VerifyCode?> GetByUserIdAndCodeAsync(Guid userId, string code);
        Task UpdateAsync(VerifyCode verifyCode);
        Task DeleteAsync(VerifyCode verifyCode);
        Task DeleteAllByUserIdAsync(Guid userId);
    }
}