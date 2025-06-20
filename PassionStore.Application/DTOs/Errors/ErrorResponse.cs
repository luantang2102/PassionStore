namespace PassionStore.Application.DTOs.Errors
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; } = string.Empty;

        public ErrorResponse(int code, string message)
        {
            Code = code;
            Message = message;
        }

        public ErrorResponse() { }
    }
}
