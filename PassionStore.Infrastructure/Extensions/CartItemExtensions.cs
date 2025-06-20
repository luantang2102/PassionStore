using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class CartItemExtensions
    {
        public static IQueryable<CartItem> Sort(this IQueryable<CartItem> query, string? orderBy)
        {
            query = orderBy switch
            {
                "dateCreatedDesc" => query.OrderByDescending(x => x.CreatedDate),
                _ => query.OrderBy(x => x.CreatedDate),
            };
            return query;
        }

    }
}
