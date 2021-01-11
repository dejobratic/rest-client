using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RestClient.Extensions
{
    public static class JsonSerializationExtensions
    {
        private static readonly JsonSerializerOptions Options =
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReferenceHandler = ReferenceHandler.Preserve
            };

        public static async Task<string> ToJsonString(
            this object data)
        {
            using var stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, data, data.GetType());
            stream.Position = 0;

            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }

        public static async Task<T> ToObject<T>(
            this string json)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes);

            return await stream.ToObject<T>();
        }

        public static async Task<T> ToObject<T>(
            this Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream, Options);
        }
    }
}
