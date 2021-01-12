namespace RestClient.Models
{
    public class RestResponse
    {
        public bool Success { get; }
        public string StatusCode { get; }
        public string Message { get; }

        public RestResponse(
            bool success,
            string statusCode,
            string message)
        {
            Success = success;
            StatusCode = statusCode;
            Message = message;
        }
    }

    public class RestResponse<T> :
        RestResponse
    {
        public T Content { get; }

        public RestResponse(
            bool success,
            string statusCode,
            string message,
            T content)
            : base(success, statusCode, message)
        {
            Content = content;
        }
    }
}
