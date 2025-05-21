using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NeuralGuideController : ControllerBase
    {
        private readonly INeuralGuideService _neuralGuideService;

        public NeuralGuideController(INeuralGuideService neuralGuideService)
        {
            _neuralGuideService = neuralGuideService;
        }

        [HttpGet("request")]
        public async Task<IActionResult> SendRequest(string request)
        {
            return Ok(await _neuralGuideService.SendRequestForGuideAsync(request));
        }
    }
}
