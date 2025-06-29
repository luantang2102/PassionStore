using PassionStore.Core.Entities;
using PassionStore.Core.Enums;
using System;
using System.Linq;

namespace PassionStore.Infrastructure.Extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<Order> Search(this IQueryable<Order> query, string? searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseTerm = searchTerm.Trim().ToLower();
            return query.Where(x => x.Status.ToString().ToLower().Contains(lowerCaseTerm)
                            || x.ShippingAddress.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.Province.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.SpecificAddress.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.Ward.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.District.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.FullName.ToLower().Contains(lowerCaseTerm)
                            || x.UserProfile.PhoneNumber.ToLower().Contains(lowerCaseTerm));
        }

        public static IQueryable<Order> Sort(this IQueryable<Order> query, string? orderBy)
        {
            query = orderBy switch
            {
                "dateAsc" => query.OrderBy(x => x.OrderDate),
                "totalAsc" => query.OrderBy(x => x.TotalAmount),
                "totalDesc" => query.OrderByDescending(x => x.TotalAmount),
                _ => query.OrderByDescending(x => x.OrderDate),
            };
            return query;
        }

        public static IQueryable<Order> Filter(this IQueryable<Order> query, string? status, string? userId)
        {
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(x => x.Status == parsedStatus);
            }
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(x => x.UserProfile.UserId.ToString() == userId);
            }

            return query;
        }
    }
}