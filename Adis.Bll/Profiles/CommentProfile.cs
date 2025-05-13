using Adis.Bll.Dtos.Comment;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Profiles
{
    public class CommentProfile : Profile
    {
        public CommentProfile() 
        {
            CreateMap<Comment, CommentDto>()
                .ForMember(dest => dest.FullNameSender,
                    opt => opt.MapFrom(src => src.Sender.FullName));

            CreateMap<PostCommentDto, Comment>();
        }
    }
}
