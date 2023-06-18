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
        public async Task<IActionResult> RetrieveToken()
        {
            try
            {
                string token = await _apiService.RetrieveTokenAsync();
                DevChallengeApiSettings.Instance.Token = token;
                return Ok();
            }
            catch (JsonException ex)
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

        [HttpGet("GetBlocksData")]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBlocksData()
        {
            try
            {
                string[] data = await _apiService.GetBlocksData();
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

        [HttpPost("Check")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Check()
        {
            try
            {
                bool result = await _apiService.CheckEncodedBlocks();
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

 