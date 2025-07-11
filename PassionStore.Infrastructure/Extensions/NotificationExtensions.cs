using PassionStore.Core.Models;
using System.Linq;

namespace PassionStore.Infrastructure.Extensions
{
    public static class NotificationExtensions
    {
        public static IQueryable<Notification> Sort(this IQueryable<Notification> query, string? orderBy)
        {
            query = orderBy switch
            {
                "createdDate_desc" => query.OrderByDescending(x => x.CreatedDate),
                "createdDate_asc" => query.OrderBy(x => x.CreatedDate),
                "content_asc" => query.OrderBy(x => x.Content),
                "content_desc" => query.OrderByDescending(x => x.Content),
                _ => query.OrderByDescending(x => x.CreatedDate), // Default: newest first
            };

            return query;
        }

        public static IQueryable<Notification> Filter(this IQueryable<Notification> query, bool? isRead)
        {
            if (isRead.HasValue)
            {
                query = query.Where(x => x.IsRead == isRead.Value);
            }

            return query;
        }
    }
}