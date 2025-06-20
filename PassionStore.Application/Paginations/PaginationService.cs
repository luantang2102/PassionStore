using Microsoft.EntityFrameworkCore;

namespace PassionStore.Application.Paginations
{
    public static class PaginationService
    {
        public static async Task<PagedList<T>> ToPagedList<T>(IQueryable<T> query, int pageNumber, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
