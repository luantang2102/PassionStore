using PassionStore.Core.Models;
using System.Linq;

namespace PassionStore.Infrastructure.Extensions
{
    public static class MessageExtensions
    {
        public static IQueryable<Message> Sort(this IQueryable<Message> query, string? orderBy)
        {
            query = orderBy switch
            {
                "createdDate_desc" => query.OrderByDescending(x => x.CreatedDate),
                "createdDate_asc" => query.OrderBy(x => x.CreatedDate),
                "content_asc" => query.OrderBy(x => x.Content),
                "content_desc" => query.OrderByDescending(x => x.Content),
                _ => query.OrderBy(x => x.CreatedDate), // Default sorting by CreatedDate ascending
            };

            return query;
        }

        public static IQueryable<Message> Search(this IQueryable<Message> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Content.ToLower().Contains(lowerCaseTerm) ||
                                    x.Id.ToString().ToLower().Contains(lowerCaseTerm) ||
                                    x.CreatedDate.ToString().Contains(lowerCaseTerm));
        }

        public static IQueryable<Message> Filter(this IQueryable<Message> query, Guid? chatId, bool? isUserMessage)
        {
            if (chatId.HasValue)
            {
                query = query.Where(x => x.ChatId == chatId.Value);
            }

            if (isUserMessage.HasValue)
            {
                query = query.Where(x => x.IsUserMessage == isUserMessage.Value);
            }

            return query;
        }
    }
}