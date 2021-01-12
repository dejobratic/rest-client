using RestClient.Models;
using System.Threading.Tasks;

namespace RestClient.Services
{
    public interface IRestService
    {
        Task<RestResponse> Send(RestRequest request);
        Task<RestResponse<TOut>> Send<TOut>(RestRequest request);
    }
}
