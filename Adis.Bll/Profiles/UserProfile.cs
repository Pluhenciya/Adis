﻿using Adis.Bll.Dtos.User;
using Adis.Dm;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
            CreateMap<User, UserDto>().ForMember(u => u.Role, opt => opt.MapFrom(u => u.Roles != null ? (Enum.Parse(typeof(Role), u.Roles.FirstOrDefault()!.Name!)) : null));
            CreateMap<UserDto, User>();
            CreateMap<PutUserDto, User>();
            CreateMap<PutUserDto, UserDto>();
        }
    }
}