using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class SizeExtensions
    {
        public static IQueryable<Size> Sort(this IQueryable<Size> query, string? orderBy)
        {
            query = orderBy switch
            {
                "nameAsc" => query.OrderBy(x => x.Name),
                "nameDesc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<Size> Search(this IQueryable<Size> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Name.ToLower().Contains(lowerCaseTerm));
        }
    }
}