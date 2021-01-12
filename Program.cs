using RestClient.Extensions;
using RestClient.Models;
using RestClient.Services;
using System;
using System.Threading.Tasks;

namespace RestClient
{
    class Program
    {
        private static readonly IRestService RestService
            = IoCContainer.Resolve<IRestService>();

        static async Task Main(string[] args)
        {
            try
            {
                var request = new RestRequest(
                    RestRequestMethod.Get,
                    "https://jsonplaceholder.typicode.com/todos");

                var response = await RestService.Send<object[]>(request);
            }
            catch (Exception ex)
            {
                var error = new { ex.Message, ex.StackTrace, ex.Source };
                Console.WriteLine(await error.ToJsonString());
            }
        }
    }
}
