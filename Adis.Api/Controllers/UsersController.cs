using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dm;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
        /// Добавляет пользователя
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
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
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
        }

        /// <summary>
        /// Возвращает список пользователей
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/users
        ///
        /// </remarks>
        /// <response code="200">Успешное выполнение</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet("{role}/{partialFullName}")]
        public async Task<IActionResult> GetUsersByPartialFullNameWithRole(string partialFullName, string role)
        {
            return Ok(await _userService.GetUsersByPartialFullNameWithRoleAsync(partialFullName, role));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            return Ok(await _userService.GetUserByIdAsync(id));
        }
    }
}
