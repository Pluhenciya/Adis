using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Bll.Services;
using Adis.Dm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять пользователями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Добавляет пользователей
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/users
        ///     {
        ///         "email": "ivan.petrov@example.com",
        ///         "passwordHash": "1234",
        ///         "role": "admin", 
        ///         "fullName": "Петров Иван Сергеевич"
        ///     }
        ///
        /// </remarks>
        /// <param name="user">Данные нового пользователя</param>
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

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                return Ok(await _userService.GetUsersAsync());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Внутренняя ошибка сервера: " + ex.Message);
            }
        }
    }
}
