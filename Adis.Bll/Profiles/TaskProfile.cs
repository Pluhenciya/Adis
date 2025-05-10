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
    public class TaskProfile : Profile
    {
        public TaskProfile() 
        { 
            CreateMap<ProjectTask, TaskDto>().ReverseMap();
        }
    }
}
