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
                string token = reponseData.RootElement.GetProperty("token").GetString() ?? throw new ArgumentNullException("Token must contain a value.");
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
        public async Task<DevChallengeBlocksData> GetBlocksData()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DevChallengeApiSettings.Instance.Token);
            HttpResponseMessage response = await _httpClient.GetAsync($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/blocks");

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                DevChallengeBlocksData? responseData = JsonSerializer.Deserialize<DevChallengeBlocksData>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

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

        /// <summary>
        /// @@@POST - check either if two blocks are sequential or if the encoded final blocks order is correct
        /// </summary>
        /// <param name="requestData"></param>
=        /// <exception cref="HttpRequestException"></exception>
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


        public async Task<bool> CheckBlocks(string block1, string block2)
        {
            var requestData = new { blocks = new string[] { block1, block2 } };
            return await SendCheckRequest(requestData);
        }

        public async Task<bool> CheckEncodedBlocks(string[] blocks)
        {
            var requestData = new { encoded = string.Join(",", blocks) };
            return await SendCheckRequest(requestData);
        }

        public async Task Check(DevChallengeBlocksData blocksData)
        {
            bool foundAnySequent = false;
            bool foundLastBlock = false;
            for (int i = 0; i < blocksData.Length - 1; i++)
            {
                // if last block was found - skip on checking the 2 last blocks
                if (foundLastBlock && i == blocksData.Length - 2) break;

                string currentBlock = blocksData.Data[i];
                for (int j = i + 1; j < blocksData.Length; j++)
                {
                    string checkBlock = blocksData.Data[j];
                    bool isSequential = await CheckBlocks(currentBlock, checkBlock);

                    // put the sequential block after the current block
                    if (isSequential)
                    {
                        Swap(blocksData.Data, i + 1, j);
                        foundAnySequent = true;
                    }
                }
                // if not found any sequent - put it as the last block, 'foundAnySequent' should be true from now on
                if (!foundAnySequent)
                {
                    Swap(blocksData.Data, i, blocksData.Length - 1);
                    foundLastBlock = true;
                }
            }
        }

        private void Swap<T>(T[] array, int index1, int index2)
        {
            T temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }
}

