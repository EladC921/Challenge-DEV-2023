﻿using Challenge_DEV_2023.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Challenge_DEV_2023.Services
{
    public class DevChallengeApiService
    {
        private string[] _blocks;
        private readonly HttpClient _httpClient;

        public string[] Blocks
        {
            get { return _blocks; }
            set { _blocks = value; }
        }

        public DevChallengeApiService(HttpClient httpClient)
        {
            _blocks = new string[0];
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// @@@POST - retrieve the bearer token 
        /// </summary>
        /// <returns>Bearer token</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<string> RetrieveTokenAsync()
        {
            var requestData = new { email = DevChallengeApiSettings.Instance.Email };
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/token", requestData);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument reponseData = JsonDocument.Parse(responseContent);
                string token = reponseData.RootElement.GetProperty("token").GetString() ?? throw new JsonException("Missing or invalid 'token' property.");
                // Save token in secrets.json
                DevChallengeApiSettings.Instance.Token = token;
                // Authorize once since this service is a singleton
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DevChallengeApiSettings.Instance.Token);
                return token;
            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }

        /// <summary>
        /// @@@GET - get the blocks data
        /// </summary>
        /// <returns>Blocks data</returns>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="HttpRequestException"></exception>
        public async Task<string[]> GetBlocksDataAsync()
        {
            if (DevChallengeApiSettings.Instance.Token == string.Empty)
            {
                await RetrieveTokenAsync();
            }

            HttpResponseMessage response = await _httpClient.GetAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/blocks");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
                // Check if "data" property exists and put its value into a variable
                if (jsonDocument.RootElement.TryGetProperty("data", out JsonElement dataElement) && dataElement.ValueKind == JsonValueKind.Array)
                {
                    _blocks = dataElement.EnumerateArray()
                        .Select(element => element.GetString())
                        .ToArray()!;
                    return _blocks;
                }

                // else
                throw new JsonException("Missing or invalid 'data' property.");

            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }

        /// <summary>
        /// @@@POST - check either if two blocks are sequential or if the encoded final blocks order is correct
        /// </summary>
        /// <param name="requestData"></param>
        /// <exception cref="HttpRequestException"></exception>
        private async Task<bool> SendCheckRequest(object requestData)
        {
            HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/check", requestData);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                JsonDocument responseData = JsonDocument.Parse(responseContent);
                bool result = responseData.RootElement.GetProperty("message").GetBoolean();
                return result;
            }
            else
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}");
            }
        }


        /// <summary>
        /// @@@Post - Check pair of blocks
        /// </summary>
        /// <param name="block1"></param>
        /// <param name="block2"></param>
        /// <returns>If pair of blocks are in sequent</returns>
        public virtual async Task<bool> CheckBlocksAsync(string block1, string block2)
        {
            if (DevChallengeApiSettings.Instance.Token == string.Empty)
            {
                await RetrieveTokenAsync();
            }
            if (_blocks.Length == 0)
            {
                await GetBlocksDataAsync();
            }

            var requestData = new { blocks = new string[] { block1, block2 } };
            return await SendCheckRequest(requestData);
        }

        /// <summary>
        /// @@@Post - Check encoded string
        /// </summary>
        /// <returns>If the encoded string is correct</returns>
        public virtual async Task<bool> CheckEncodedBlocksAsync()
        {
            if (DevChallengeApiSettings.Instance.Token == string.Empty)
            {
                await RetrieveTokenAsync();
            }
            if (_blocks.Length == 0)
            {
                await GetBlocksDataAsync();
            }

            var requestData = new { encode = string.Join("", await Check()) };
            return await SendCheckRequest(requestData);
        }

        public async Task<string[]> Check()
        {
            if (_blocks.Length == 0) throw new ArgumentNullException("Invlaid action: Blocks data must be retrieved before applying this method.");

            string[] sortedBlocks = (string[])_blocks.Clone();
            int sortedBlocksLength = sortedBlocks.Length;
            bool foundAnySequent = false;

            for (int i = 0; i < sortedBlocksLength - 1; i++)
            {
                string currentBlock = sortedBlocks[i];
                for (int j = i + 1; j < sortedBlocksLength; j++)
                {
                    string checkBlock = sortedBlocks[j];
                    // Put the sequential block after the current block
                    if (await CheckBlocksAsync(currentBlock, checkBlock))
                    {
                        Swap(sortedBlocks, i + 1, j);
                        foundAnySequent = true;
                        break;
                    }
                }
                // Put the last block in its place
                if (!foundAnySequent)
                {
                    Swap(sortedBlocks, i, sortedBlocksLength - 1);
                    sortedBlocksLength--; // No need to check the last block
                }
            }

            return sortedBlocks;
        }

        private void Swap<T>(T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }
}

