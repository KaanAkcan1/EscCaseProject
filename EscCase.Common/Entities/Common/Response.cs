namespace EscCase.Common.Entities.Common
{
    public class Response
    {
        public string Message { get; set; } = string.Empty;

        public bool Success { get; set; } = false;

        public int StatusCode { get; set; } = 200;

        public IEnumerable<ApiError>? Error { get; set; } = null;

        public void SetError(string message, int? statusCode = 500)
        {
            Success = false;
            StatusCode = statusCode ?? 500;
            Message = Message == string.Empty ? message : Message + " - " + message;
        }

        public void SetSuccess(string? message = "", int? statusCode = 200)
        {
            Success = true;
            StatusCode = statusCode ?? 200;
            Message = message ?? "Operation Successful";
        }
    }
}
