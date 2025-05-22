using Adis.Bll.Dtos;
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

        [HttpPost()]
        public async Task<IActionResult> SendRequest(NeuralRequest request)
        {
            return Ok(new { 
                Answer = await _neuralGuideService.SendRequestForGuideAsync(request.Message)
            });
        }
    }
}
