using System;
using Challenge_DEV_2023.Models;
using Microsoft.Extensions.Options;

namespace Challenge_DEV_2023.Services
{
    public class DevChallengeApiService
    {
        private readonly HttpClient _httpClient;
        private readonly DevChallengeApiSettings _apiSettings;

        public DevChallengeApiService(HttpClient httpClient, IOptions<DevChallengeApiSettings> apiSettingsOptions)
        {
            _httpClient = httpClient;
            _apiSettings = apiSettingsOptions.Value;
        }

        public async Task<string> GetTokenAsync(string email)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"{_apiSettings.BaseUrl}/token?email={email}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }
    }
}

