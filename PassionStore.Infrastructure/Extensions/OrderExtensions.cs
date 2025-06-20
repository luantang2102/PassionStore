using PassionStore.Core.Models;

namespace PassionStore.Infrastructure.Extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<Order> Sort(this IQueryable<Order> query, string? orderBy)
        {
            query = orderBy switch
            {
                "dateDesc" => query.OrderByDescending(x => x.OrderDate),
                "totalAsc" => query.OrderBy(x => x.TotalAmount),
                "totalDesc" => query.OrderByDescending(x => x.TotalAmount),
                _ => query.OrderBy(x => x.OrderDate),
            };
            return query;
        }

        public static IQueryable<Order> Filter(this IQueryable<Order> query, string? status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(x => x.Status.ToLower() == status.ToLower());
            }
            return query;
        }
    }
}