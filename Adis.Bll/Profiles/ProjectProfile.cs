using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adis.Bll.Dtos;
using Adis.Dm;
using AutoMapper;

namespace Adis.Bll.Profiles
{
    /// <summary>
    /// Профиль для настройки AutoMapper для проектов
    /// </summary>
    public class ProjectProfile : Profile
    {
        public ProjectProfile() 
        {
            CreateMap<PostProjectDto, Project>().ReverseMap();
        }
    }
}
