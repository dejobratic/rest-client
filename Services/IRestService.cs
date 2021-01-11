using RestClient.Models;
using System.Threading.Tasks;

namespace RestClient.Services
{
    public interface IRestService
    {
        Task Send(RestRequest request);
        Task<TOut> Send<TOut>(RestRequest request);
    }
}
