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

        public CommentService(IMapper mapper, IHttpContextAccessor contextAccessor, ICommentRepository commentRepository)
        {
            _mapper = mapper;
            _contextAccessor = contextAccessor;
            _commentRepository = commentRepository;
        }

        public async Task<CommentDto> AddCommentAsync(PostCommentDto commentDto)
        {
            var comment = _mapper.Map<Comment>(commentDto);
            comment.IdSender = int.Parse(_contextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            int idComment = (await _commentRepository.AddAsync(comment)).IdComment;
            return _mapper.Map<CommentDto>(await _commentRepository.GetCommentByIdAsync(idComment));
        }
    }
}
