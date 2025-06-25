using PassionStore.Core.Entities;

namespace PassionStore.Infrastructure.Extensions
{
    public static class UserProfileExtensions
    {
        public static IQueryable<UserProfile> Sort(this IQueryable<UserProfile> query, string? orderBy)
        {
            query = orderBy switch
            {
                "name_asc" => query.OrderBy(x => x.FullName),
                "name_desc" => query.OrderByDescending(x => x.FullName),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<UserProfile> Search(this IQueryable<UserProfile> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.FullName.ToLower().Contains(lowerCaseTerm) ||
                                    x.PhoneNumber.ToLower().Contains(lowerCaseTerm));
        }
    }
}
