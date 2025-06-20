namespace PassionStore.Core.Interfaces.IRepositories;

public interface IUnitOfWork
{
    public Task<int> CommitAsync();
}