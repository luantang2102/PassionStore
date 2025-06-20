using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class CategoryExtensions
    {
        public static IQueryable<Category> Sort(this IQueryable<Category> query, string? orderBy)
        {
            query = orderBy switch
            {
                "dateCreatedDesc" => query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<Category> Search(this IQueryable<Category> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Name.ToLower().Contains(lowerCaseTerm)
            || x.Description.ToLower().Contains(lowerCaseTerm)
            || x.Level.ToString().Contains(lowerCaseTerm)
            || x.IsActive && "active".Contains(lowerCaseTerm)
            || x.ParentCategory.Name.ToLower().Contains(lowerCaseTerm)
            || x.CreatedDate.ToString()!.Contains(lowerCaseTerm)
            || x.UpdatedDate != null && x.UpdatedDate.ToString()!.Contains(lowerCaseTerm));
        }

        public static IQueryable<Category> Filter(this IQueryable<Category> query, string? level, bool? isActive)
        {
            if (!string.IsNullOrEmpty(level))
            {
                var levelValue = int.TryParse(level, out var parsedLevel) ? parsedLevel : 0;
                query = query.Where(x => x.Level == levelValue);
            }
            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }
            return query;
        }

    }
}
