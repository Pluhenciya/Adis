using Adis.Bll.Dtos.User;
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
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpPost]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin")]
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
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        /// <summary>
        /// Возвращает список пользователей указанной роли и части его ФИО
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/users/projecter/Попо
        ///
        /// </remarks>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpGet("{role}/{partialFullName}")]
        [ProducesResponseType(typeof(IEnumerable<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> GetUsersByPartialFullNameWithRole(string partialFullName, string role)
        {
            return Ok(await _userService.GetUsersByPartialFullNameWithRoleAsync(partialFullName, role));
        }

        /// <summary>
        /// Возвращает пользователя по идентификатору
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     GET /api/users/1
        ///
        /// </remarks>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        [Authorize(Roles = "Admin, ProjectManager")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound("Пользователь с таким id не найден");
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser(PutUserDto user)
        {
            return Ok(await _userService.UpdateUserAsync(user));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }
    }
}
