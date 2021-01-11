using RestClient.Extensions;
using RestClient.Models;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RestClient.Services
{
    public class RestService :
        IRestService
    {
        private readonly IHttpClientFactory _clientFactory;

        public RestService(
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory ??
                throw new ArgumentNullException(nameof(clientFactory));
        }

        public async Task Send(RestRequest request)
        {
            HttpRequestMessage httpRequest = await CreateHttpRequest(request);
            await Send(httpRequest);
        }

        public async Task<TOut> Send<TOut>(
            RestRequest request)
        {
            HttpRequestMessage httpRequest = await CreateHttpRequest(request);
            HttpResponseMessage httpResponse = await Send(httpRequest);
            return await Resolve<TOut>(httpResponse);
        }

        private async Task<HttpRequestMessage> CreateHttpRequest(
            RestRequest request)
        {
            HttpRequestMessage httpRequest = CreateBaseHttpRequest(request);
            AddHeaders(httpRequest, request.Headers);
            await AddBody(httpRequest, request.Body);

            return httpRequest;
        }

        private static HttpRequestMessage CreateBaseHttpRequest(
            RestRequest request)
        {
            var httpRequest = new HttpRequestMessage(
                ResolveHttpMethod(request.Method),
                request.Url);

            httpRequest.Headers.Add("Accept", "application/json");

            return httpRequest;
        }

        private static void AddHeaders(
            HttpRequestMessage httpRequest, NameValueCollection headers)
        {
            if (headers is null) return;

            foreach (string headerName in headers.AllKeys)
                httpRequest.Headers.Add(headerName, headers[headerName]);
        }

        private static async Task AddBody<TIn>(
            HttpRequestMessage httpRequest, TIn body)
        {
            if (body is null) return;

            string json = await body.ToJsonString();
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static HttpMethod ResolveHttpMethod(string method)
        {
            switch (method)
            {
                case RestRequestMethod.Get: return HttpMethod.Get;
                case RestRequestMethod.Put: return HttpMethod.Put;
                case RestRequestMethod.Post: return HttpMethod.Post;
                case RestRequestMethod.Delete: return HttpMethod.Delete;

                default: throw new Exception($"Unable to resolve http method from {method}.");
            }
        }

        private async Task<HttpResponseMessage> Send(
            HttpRequestMessage httpRequest)
        {
            HttpClient client = _clientFactory.CreateClient();
            HttpResponseMessage httpResponse = await client.SendAsync(httpRequest);

            if (httpResponse.IsSuccessStatusCode)
                return httpResponse;

            throw new Exception($"Error sending request from HTTP service. Reason: {httpResponse.ReasonPhrase}.");
        }

        private static async Task<TOut> Resolve<TOut>(
            HttpResponseMessage response)
        {
            using Stream stream = await response.Content.ReadAsStreamAsync();
            if (stream.Length == 0) return default;

            return await stream.ToObject<TOut>();
        }
    }
}
