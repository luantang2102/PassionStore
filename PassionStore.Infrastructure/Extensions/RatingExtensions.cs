using PassionStore.Core.Entities;

namespace PassionStore.Infrastructure.Extensions
{
    public static class RatingExtensions
    {
        public static IQueryable<Rating> Sort(this IQueryable<Rating> query, string? orderBy)
        {
            query = orderBy switch
            {
                "dateCreatedDesc" => query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

        public static IQueryable<Rating> Filter(this IQueryable<Rating> query, string? value, string? hasComment)
        {
            if (!string.IsNullOrEmpty(value))
            {
                var ratingValue = double.TryParse(value, out var parsedValue) ? parsedValue : 0;
                query = query.Where(x => x.Value == ratingValue);
            }
            if (!string.IsNullOrEmpty(hasComment))
            {
                var hasCommentValue = bool.TryParse(hasComment, out var parsedHasComment) ? parsedHasComment : false;
                query = query.Where(x => x.Comment != null && hasCommentValue);
            }
            return query;
        }

    }
}
