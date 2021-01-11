using System.Collections.Specialized;

namespace RestClient.Models
{
    public class RestRequest
    {
        public string Url { get; }
        public string Method { get; }
        public object Body { get; }
        public NameValueCollection Headers { get; }
        public RestRequest(
            string method,
            string url,
            object body = default,
            NameValueCollection headers = default)
        {
            Method = method;
            Url = url;
            Body = body;
            Headers = headers;

            //ThrowIfInvalid();
        }
    }
}
