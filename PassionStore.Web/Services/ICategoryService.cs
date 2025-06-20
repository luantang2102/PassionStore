using PassionStore.Web.Models.Views;

namespace PassionStore.Web.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryView>> GetCategoriesTreeAsync(CancellationToken cancellationToken);
    }
}
