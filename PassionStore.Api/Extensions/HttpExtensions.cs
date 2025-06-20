using Microsoft.Net.Http.Headers;
using PassionStore.Application.Paginations;
using System.Text.Json;

namespace PassionStore.Api.Extensions
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, PaginationMetadata metadata)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Append("Pagination", JsonSerializer.Serialize(metadata, options));
            response.Headers.Append(HeaderNames.AccessControlExposeHeaders, "Pagination");
        }
    }
}
