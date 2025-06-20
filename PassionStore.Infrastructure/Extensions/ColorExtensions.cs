using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class ColorExtensions
    {
        public static IQueryable<Color> Sort(this IQueryable<Color> query, string? orderBy)
        {
            query = orderBy switch
            {
                "nameAsc" => query.OrderBy(x => x.Name),
                "nameDesc" => query.OrderByDescending(x => x.Name),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<Color> Search(this IQueryable<Color> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Name.ToLower().Contains(lowerCaseTerm) || x.HexCode.ToLower().Contains(lowerCaseTerm));
        }
    }
}