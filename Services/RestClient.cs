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

        public async Task<RestResponse> Send(RestRequest request)
        {
            HttpRequestMessage httpRequest = await CreateHttpRequest(request);
            HttpResponseMessage httpResponse = await Send(httpRequest);
            return ResolveResponse(httpResponse);
        }

        public async Task<RestResponse<TOut>> Send<TOut>(
            RestRequest request)
        {
            HttpRequestMessage httpRequest = await CreateHttpRequest(request);
            HttpResponseMessage httpResponse = await Send(httpRequest);
            return await ResolveResponse<TOut>(httpResponse);
        }

        private static async Task<HttpRequestMessage> CreateHttpRequest(
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
            return await client.SendAsync(httpRequest);
        }

        private static RestResponse ResolveResponse(
            HttpResponseMessage response)
        {
            return new RestResponse(
                response.IsSuccessStatusCode,
                response.StatusCode.ToString(),
                response.ReasonPhrase);
        }

        private static async Task<RestResponse<TOut>> ResolveResponse<TOut>(
            HttpResponseMessage response)
        {
            return new RestResponse<TOut>(
                response.IsSuccessStatusCode,
                response.StatusCode.ToString(),
                response.ReasonPhrase,
                await ResolveResponseContent<TOut>(response.Content));
        }

        private static async Task<TOut> ResolveResponseContent<TOut>(
            HttpContent content)
        {
            using Stream contentStream = await content.ReadAsStreamAsync();
            if (contentStream.Length == 0) return default;

            return await contentStream.ToObject<TOut>();
        }
    }
}
