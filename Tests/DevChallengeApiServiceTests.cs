using NUnit.Framework;
using Moq;
using Challenge_DEV_2023.Services;
using System.Net;
using Moq.Protected;
using System.Text.Json;
using Challenge_DEV_2023.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Challenge_DEV_2023.Tests
{
    public class DevChallengeApiServiceTests
    {
        private string[] _expectedBlocks;

        [OneTimeSetUp]
        public void Setup()
        {
            // Set the blocks wanted result manually
            _expectedBlocks = new[] { "A", "B", "C", "D", "E" };
        }

        /// <summary>
        /// Functional Test - Actual API calls
        /// </summary>
        [Test]
        public async Task Check_ApiValidResponses()
        {
            // Arrange
            var apiService = new DevChallengeApiService(new HttpClient());

            // Act - apply the api calls
            bool testResult = await apiService.CheckEncodedBlocksAsync();

            // Assert
            Assert.IsTrue(testResult);
        }

        /// <summary>
        /// Unit Test - Simulated API calls to check the functionality of the Service
        /// </summary>
        [Test]
        public async Task Check_ValidResponses_ReturnsBlocksArraysComparison_ReturnsEncodedBlocksComparison()
        {
            // Arrange
            var mockHttp = SetupMockHttp();
            var httpClient = new HttpClient(mockHttp.Object);
            var apiService = new DevChallengeApiService(httpClient);

            // Act
            await apiService.GetBlocksDataAsync();

            string[] sortedBlocks = await apiService.Check();

            bool testFinalResult = await apiService.CheckEncodedBlocksAsync();

            // Assert
            Assert.AreEqual(_expectedBlocks, sortedBlocks);
            Assert.IsTrue(testFinalResult);
        }

        // Set up the mock behavior for the API requests
        private Mock<HttpMessageHandler> SetupMockHttp()
        {
            var mockHttp = new Mock<HttpMessageHandler>();

            // for RetrieveToken
            mockHttp
               .Protected()
               .Setup<Task<HttpResponseMessage>>(
                   "SendAsync",
                   ItExpr.Is<HttpRequestMessage>(req =>
                       req.Method == HttpMethod.Post &&
                       req.RequestUri.ToString().Equals($"{DevChallengeApiSettings.Instance.BaseUrl}/token")
                   ),
                   ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"token\": \"someToken\" }"),
               });


            // for GetBlocksAsync
            mockHttp
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Equals($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/blocks")
                    ),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": [\"A\",\"E\",\"D\",\"C\",\"B\"]}"),
                });

            // for CheckEncodedBlocks and CheckBlocks
            mockHttp
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Equals($"{DevChallengeApiSettings.Instance.BaseUrl}/v1/check")
                     ),
                     ItExpr.IsAny<CancellationToken>())
                .Returns((HttpRequestMessage request, CancellationToken cancellationToken) =>
                {
                    var requestContent = request.Content.ReadAsStringAsync().Result;
                    var requestData = JObject.Parse(requestContent);
                    var messageData = false;
                    if (requestData.ContainsKey("blocks"))
                    {
                        var block1 = requestData["blocks"][0].ToString();
                        var block2 = requestData["blocks"][1].ToString();
                        messageData = IsInSequence(block1, block2);
                    }
                    else if (requestData.ContainsKey("encode"))
                    {
                        var encode = requestData["encode"].ToString();
                        messageData = encode.Equals(string.Join("", _expectedBlocks));
                    }
                    else
                    {
                        throw new ArgumentException("Invalid request data. Missing 'blocks' or 'encode' property.");
                    }

                    var responseContent = new { message = messageData };
                    var response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(responseContent))
                    };

                    return Task.FromResult(response);
                });

            return mockHttp;
        }

        private bool IsInSequence(string block1, string block2)
        {
            for (int i = 0; i < _expectedBlocks.Length; i++)
            {
                if (_expectedBlocks[i] == block1)
                {
                    if (_expectedBlocks[i + 1] == block2)
                        return true;
                    break;
                }
            }
            return false;
        }
    }
}

