using System.Text.Json;
using Challenge_DEV_2023.Models;
using Challenge_DEV_2023.Services;
using Microsoft.AspNetCore.Mvc;

namespace Challenge_DEV_2023.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DevChallengeController : Controller
    {
        private readonly DevChallengeApiService _apiService;

        public DevChallengeController(DevChallengeApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost("RetrieveToken")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post()
        {
            try
            {
                string token = await _apiService.RetrieveTokenAsync();
                DevChallengeApiSettings.Instance.Token = token;
                return Ok();
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error in HTTP request: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("GetBlocksData")]
        [ProducesResponseType(typeof(DevChallengeBlocksData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get()
        {
            try
            {
                DevChallengeBlocksData data = await _apiService.GetBlocksData();
                return Ok(data);
            }
            catch(JsonException ex)
            {
                return BadRequest("Invalid JSON format: " + ex.Message);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error in HTTP request: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpPost("CheckBlocks")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post(string block1, string block2)
        {
            try
            {
                bool result = await _apiService.CheckBlocks(block1, block2);
                return Ok(result);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error in HTTP request: " + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }
    }
}

 