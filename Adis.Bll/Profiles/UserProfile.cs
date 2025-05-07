using Adis.Bll.Dtos;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Profiles
{
    /// <summary>
    /// Профиль для настройки AutoMapper для пользователей
    /// </summary>
    public class UserProfile : Profile
    {
        public UserProfile() 
        {
            CreateMap<User, UserDto>().ForMember("Role", opt => opt.MapFrom(u => (Role)(u.Roles.FirstOrDefault()!.Id - 1)));
            CreateMap<UserDto, User>();
        }
    }
}
