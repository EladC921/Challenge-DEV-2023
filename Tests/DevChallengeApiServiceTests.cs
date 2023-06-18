using NUnit.Framework;
using Moq;
using Challenge_DEV_2023.Services;

namespace Challenge_DEV_2023.Tests
{
    public class DevChallengeApiServiceTests
    {
        private Mock<DevChallengeApiService> _apiServiceMock;
        private string[] _expectedBlocks;

        [OneTimeSetUp]
        public void Setup()
        {
            // Create a mock of the ApiService
            _apiServiceMock = new Mock<DevChallengeApiService>(new HttpClient());

            // Set the blocks manually
            _apiServiceMock.Object.Blocks = new[] { "A", "E", "B", "D", "C" };
            _expectedBlocks = new[] { "A", "B", "C", "D", "E" };

            // Mock the CheckBlocks method to use the isInSequence function for checking blocks
            _apiServiceMock
                .Setup(x => x.CheckBlocks(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string block1, string block2) => Task.FromResult(isInSequence(block1, block2)));

            // Mock the CheckEncodedBlocks method to use the isEncodedCorrect function for checking final result
            _apiServiceMock
                .Setup(x => x.CheckEncodedBlocks())
                .Returns(Task.FromResult(isEncodedCorrect()));
        }

        [Test]
        public async Task Check_Test()
        {
            // Arrange        
            var mock = _apiServiceMock.Object;

            // Act
            var result = await mock.CheckEncodedBlocks();

            // Assert
            Assert.IsTrue(result);
        }

        private bool isInSequence(string block1, string block2)
        {
            for (int i = 0; i < _expectedBlocks.Length; i++)
            {
                if (_expectedBlocks[i] == block1)
                {
                    if (_expectedBlocks[i + 1] == block2)
                        return true;
                }
                else break;
            }
            return false;
        }

        private bool isEncodedCorrect()
        {
            return _expectedBlocks.SequenceEqual(_apiServiceMock.Object.Blocks);
        }
    }
}

