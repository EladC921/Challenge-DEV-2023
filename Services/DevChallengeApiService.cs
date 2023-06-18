using Challenge_DEV_2023.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Challenge_DEV_2023.Services
{
    public class DevChallengeApiService
    {
        private string[] _blocks;
        private readonly HttpClient _httpClient;

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
        public async Task<string[]> GetBlocksData()
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DevChallengeApiSettings.Instance.Token);
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", DevChallengeApiSettings.Instance.Token);
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
        public async Task<bool> CheckBlocks(string block1, string block2)
        {
            var requestData = new { blocks = new string[] { block1, block2 } };
            return await SendCheckRequest(requestData);
        }

        /// <summary>
        /// @@@Post - Check encoded string
        /// </summary>
        /// <returns>If the encoded string is correct</returns>
        public async Task<bool> CheckEncodedBlocks()
        {
            var requestData = new { encode = string.Join("", await Check()) };
            return await SendCheckRequest(requestData);
        }

        public async Task<string[]> Check()
        {
            string[] sortedBlocks = (string[])_blocks.Clone();
            bool foundAnySequent = false;
            bool foundLastBlock = false;
            int blocksLenghToCheck = sortedBlocks.Length;
            int lastBlockToCheckIndex = blocksLenghToCheck - 1;

            for (int i = 0; i < blocksLenghToCheck - 1; i++)
            {
                string currentBlock = sortedBlocks[i];
                for (int j = i + 1; j < blocksLenghToCheck; j++)
                {
                    string checkBlock = sortedBlocks[j];
                    // Check if every block is sequent to the current or to the last
                    if (foundLastBlock)
                    {
                        string lastBlockToCheck = sortedBlocks[lastBlockToCheckIndex];
                        if (await CheckBlocks(checkBlock, lastBlockToCheck))
                        {
                            Swap(sortedBlocks, j, lastBlockToCheckIndex - 1);
                            // Update the current last block to check sequent
                            lastBlockToCheckIndex--;
                            blocksLenghToCheck--;
                        }
                    }
                    // Put the sequential block after the current block
                    if (await CheckBlocks(currentBlock, checkBlock))
                    {
                        Swap(sortedBlocks, i + 1, j);
                        foundAnySequent = true;
                        break;
                    }
                }
                // Get into this condition once -> put the last block in its place
                if (!foundAnySequent)
                {
                    Swap(sortedBlocks, i, blocksLenghToCheck - 1);
                    foundLastBlock = true;
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

