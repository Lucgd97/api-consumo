using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Consumo_Api_Lacuna_Genetics.Class
{
    public class ApiHelper
    {
        private readonly string _baseAddress;

        public ApiHelper(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        public async Task<HttpResponseMessage> PostAsync(object request, string uri, string? accessToken = null)
        {
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            if (accessToken != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var requestJson = JsonConvert.SerializeObject(request);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync(uri, content);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error trying to call the api to uri: {uri}. Details: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;

                return null;
            }

        }

        public async Task<HttpResponseMessage> GetAsync(string uri, string? accessToken = null)
        {
            using var client = new HttpClient { BaseAddress = new Uri(_baseAddress) };
            if (accessToken != null)
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            try
            {
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();

                return response;
            }
            catch (Exception ex)
            {

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error trying to call the api to uri: {uri}. Details: {ex.Message}");
                Console.ForegroundColor = ConsoleColor.Gray;

                return null;
            }
        }
    }
}
