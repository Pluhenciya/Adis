using Adis.Bll.Dtos.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface ICommentService
    {
        public Task<CommentDto> AddCommentAsync(PostCommentDto commentDto);
    }
}
