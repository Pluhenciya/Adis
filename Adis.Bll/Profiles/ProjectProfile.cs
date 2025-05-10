using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adis.Bll.Dtos.Project;
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
            CreateMap<PostProjectDto, Project>()
                .ForMember(dest => dest.Contractor,
                    opt => opt.MapFrom(src => src.ContractorName != null ? new Contractor
                    {
                        Name = src.ContractorName
                    } : null));
            
            CreateMap<Project, GetProjectDto>()
                .ForMember(dest => dest.ResponsiblePerson,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Progress,
                    opt => opt.MapFrom(src => CalculateProgress(src)))
                .ForMember(dest => dest.ContractorName,
                    opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : null));

            CreateMap<Project, GetProjectWithTasksDto>()
                .ForMember(dest => dest.ResponsiblePerson,
                    opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Progress,
                    opt => opt.MapFrom(src => CalculateProgress(src)))
                .ForMember(dest => dest.ContractorName,
                    opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Name : null));
        }

        private static int CalculateProgress(Project project)
        {
            if (project.Tasks == null || !project.Tasks.Any())
                return 0;

            var totalTasks = project.Tasks.Count();
            var completedTasks = project.Tasks.Count(t => t.Status == Status.Completed);

            return (int)Math.Round((double)completedTasks / totalTasks * 100);
        }
    }
}
