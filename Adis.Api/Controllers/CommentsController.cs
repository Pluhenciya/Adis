using Adis.Bll.Dtos.Comment;
using Adis.Bll.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Adis.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService) 
        {
            _commentService = commentService;
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(PostCommentDto comment)
        {
            return Ok(await _commentService.AddCommentAsync(comment));
        }
    }
}
