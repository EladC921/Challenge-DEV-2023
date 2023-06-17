using Challenge_DEV_2023.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Challenge_DEV_2023.Controllers
{
    public class DevChallengeController : Controller
    {
        private readonly DevChallengeApiService _apiService;
        private readonly IConfiguration _config;

        public DevChallengeController(DevChallengeApiService apiService, IConfiguration config)
        {
            _apiService = apiService;
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
           string email = _config["DevChallengeApiSettings:Email"]!;

            string token = await _apiService.GetTokenAsync(email);

            _config["DevChallengeApiSettings:Token"] = token;


            return Ok(token);
        }
    }
}

