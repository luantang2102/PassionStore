using PassionStore.Core.Entities;

namespace PassionStore.Infrastructure.Extensions
{
    public static class ProductExtensions
    {
        public static IQueryable<Product> Sort(this IQueryable<Product> query, string? orderBy)
        {
            query = orderBy switch
            {
                "popularity" => query.OrderByDescending(x => x.AverageRating),
                "dateCreated_desc" => query.OrderByDescending(x => x.CreatedDate),
                "dateCreated_asc" => query.OrderBy(x => x.CreatedDate),
                "name_asc" => query.OrderBy(x => x.Name),
                "name_desc" => query.OrderByDescending(x => x.Name),
                "price_asc" => query.OrderBy(x => x.MinPrice),
                "price_desc" => query.OrderByDescending(x => x.MaxPrice),
                "averageRating_asc" => query.OrderBy(x => x.AverageRating),
                "averageRating_desc" => query.OrderByDescending(x => x.AverageRating),

                _ => query.OrderBy(x => x.CreatedDate),
            };

            return query;
        }

        public static IQueryable<Product> Search(this IQueryable<Product> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Name.ToLower().Contains(lowerCaseTerm)
                || x.Description.ToLower().Contains(lowerCaseTerm)
                || x.Categories.Any(y => y.Name.ToLower().Contains(lowerCaseTerm))
                || x.CreatedDate.ToString()!.Contains(lowerCaseTerm)
                || x.UpdatedDate != null && x.UpdatedDate.ToString()!.Contains(lowerCaseTerm)
                || x.Id.ToString().ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Product> Filter(this IQueryable<Product> query, string? categories, string? ratings, string? minPrice, string? maxPrice, bool? isFeatured)
        {
            var categoryList = new List<string>();
            var ratingList = new List<double>();

            if (!string.IsNullOrEmpty(categories))
            {
                categoryList.AddRange(categories.ToLower().Split(",").ToList());

                query = query.Where(x => x.Categories.Any(c => categoryList.Contains(c.Id.ToString().ToLower())));
            }

            if (isFeatured.HasValue)
            {
                query = query.Where(x => x.IsFeatured == isFeatured.Value);
            }

            return query;
        }


    }
}
