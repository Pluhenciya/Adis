using Adis.Bll.Dtos.Auth;
using Adis.Bll.Dtos.Comment;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Adis.Api.Controllers
{
    /// <summary>
    /// Позволяет управлять комментариями
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, ProjectManager, Projecter")]
    public class CommentsController(ICommentService commentService) : ControllerBase
    {
        /// <inheritdoc cref="ICommentService"/>
        private readonly ICommentService _commentService = commentService;

        /// <summary>
        /// Создает новый комментарий
        /// </summary>
        /// <remarks>
        /// Пример запроса:
        ///
        ///     POST /api/comments
        ///     {
        ///         "idTask": 123,
        ///         "text": "Это пример текста комментария"
        ///     }
        ///
        /// </remarks>
        /// <param name="comment">Данные нового комментария</param>
        /// <response code="200">Успешное выполнение</response>
        /// <response code="400">Ошибка валидации данных</response>
        /// <response code="401">Неавторизованный пользователь</response>
        /// <response code="403">Пользователь без прав на это действие</response>
        [HttpPost]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.Forbidden)]
        public async Task<IActionResult> AddComment(PostCommentDto comment)
        {
            try
            {
                return Ok(await _commentService.AddCommentAsync(comment));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
