namespace PassionStore.Application.DTOs.Errors
{
    public class ErrorWithBodyResponse<T> : ErrorResponse
    {
        public T Body { get; set; } = default!;

        public ErrorWithBodyResponse(int code, string message, T body = default!) : base(code, message)
        {
            Body = body;
        }
    }
}
