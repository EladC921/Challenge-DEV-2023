using Challenge_DEV_2023.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Challenge_DEV_2023.Services
{
    public class DevChallengeApiService
    {
        private readonly HttpClient _httpClient;

        public DevChallengeApiService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> RetrieveTokenAsync()
        {
            var requestData = new { email = DevChallengeApiSettings.Instance.Email };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/token", requestData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument reponseData = JsonDocument.Parse(responseContent);
                string token = reponseData.RootElement.GetProperty("token").GetString() ?? throw new ArgumentNullException("Token must contain a value.");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DevChallengeApiSettings.Instance.Token);
                return token;
            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }

        public async Task<DevChallengeBlocksData> GetBlocksData()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/blocks");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                DevChallengeBlocksData? responseData = JsonSerializer.Deserialize<DevChallengeBlocksData>(responseContent);

                // Check if any required property is null and throw an exception
                if (responseData?.Data == null || responseData.ChunkSize == 0 || responseData.Length == 0)
                {
                    throw new JsonException("Missing required properties.");
                }

                return responseData;
            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }
    }
}

