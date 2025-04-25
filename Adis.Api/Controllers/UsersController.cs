using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Bll.Services;
using Adis.Dm;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpPost]
        public async Task<IActionResult> AddUser(UserDto user)
        {
            try
            {
                return Ok(await _userService.AddUserAsync(user));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутренняя ошибка сервера: " + ex.Message);
            }
        }
    }
}
