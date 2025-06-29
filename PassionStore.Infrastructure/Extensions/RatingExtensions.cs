using PassionStore.Core.Entities;
using System.Linq;
using System.Collections.Generic;

namespace PassionStore.Infrastructure.Extensions
{
    public static class RatingExtensions
    {
        public static IQueryable<Rating> Sort(this IQueryable<Rating> query, string? orderBy)
        {
            query = orderBy switch
            {
                "value_asc" => query.OrderBy(x => x.Value),
                "value_desc" => query.OrderByDescending(x => x.Value),
                "dateCreated_asc" => query.OrderBy(x => x.CreatedDate),
                "dateCreated_desc" => query.OrderByDescending(x => x.CreatedDate),
                "helpful_asc" => query.OrderBy(x => x.Helpful),
                "helpful_desc" => query.OrderByDescending(x => x.Helpful),
                _ => query.OrderBy(x => x.CreatedDate),
            };

            return query;
        }

        public static IQueryable<Rating> Search(this IQueryable<Rating> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Comment!.ToLower().Contains(lowerCaseTerm)
                || x.User.UserName.ToLower().Contains(lowerCaseTerm)
                || x.Product.Name.ToLower().Contains(lowerCaseTerm)
                || x.CreatedDate.ToString()!.Contains(lowerCaseTerm)
                || x.UpdatedDate != null && x.UpdatedDate.ToString()!.Contains(lowerCaseTerm)
                || x.Id.ToString().ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Rating> Filter(this IQueryable<Rating> query, List<int>? values, string? hasComment)
        {
            if (values != null && values.Any())
            {
                query = query.Where(x => values.Contains(x.Value));
            }

            if (!string.IsNullOrEmpty(hasComment))
            {
                if (bool.TryParse(hasComment, out var parsedHasComment))
                {
                    query = query.Where(x => parsedHasComment ? !string.IsNullOrEmpty(x.Comment) : string.IsNullOrEmpty(x.Comment));
                }
            }

            return query;
        }
    }
}