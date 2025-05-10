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
            CreateMap<User, UserDto>().ForMember(u => u.Role, opt => opt.MapFrom(u => Enum.Parse(typeof(Role), u.Roles.FirstOrDefault()!.Name!)));
            CreateMap<UserDto, User>();
        }
    }
}