using Adis.Bll.Dtos.Comment;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ICommentRepository _commentRepository;
        private readonly ITaskService _taskService;

        public CommentService(IMapper mapper, IHttpContextAccessor contextAccessor, ICommentRepository commentRepository, ITaskService taskService)
        {
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _commentRepository = commentRepository;
            _taskService = taskService;
        }

        public async Task<CommentDto> AddCommentAsync(PostCommentDto commentDto)
        {
            if (await _taskService.TaskExistAsync(commentDto.IdTask))
                throw new KeyNotFoundException($"Задача с id {commentDto.IdTask} не найдена");

            var comment = _mapper.Map<Comment>(commentDto);
            comment.IdSender = int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            int idComment = (await _commentRepository.AddAsync(comment)).IdComment;
            return _mapper.Map<CommentDto>(await _commentRepository.GetCommentByIdAsync(idComment));
        }
    }
}
