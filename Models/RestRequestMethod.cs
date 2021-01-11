using System.Collections.Generic;

namespace RestClient.Models
{
    public static class RestRequestMethod
    {
        public const string Get = "GET";
        public const string Put = "PUT";
        public const string Post = "POST";
        public const string Delete = "DELETE";

        public static IEnumerable<string> GetAllowedMethods()
        {
            yield return Get;
            yield return Put;
            yield return Post;
            yield return Delete;
        }
    }
}
