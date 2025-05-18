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
            if (project.Status == ProjectStatus.Designing && (project.Tasks == null || !project.Tasks.Any()))
                return 0;

            if (project.Status == ProjectStatus.InExecution && (project.ExecutionTasks == null || !project.ExecutionTasks.Any()))
                return 0;

            int totalTasks;
            int completedTasks;

            if(project.Status == ProjectStatus.Designing)
            {
                totalTasks = project.Tasks.Count();
                completedTasks = project.Tasks.Count(t => t.Status == Status.Completed);
            }
            else if (project.Status == ProjectStatus.InExecution)
            {
                totalTasks = project.ExecutionTasks.Count();
                completedTasks = project.ExecutionTasks.Count(t => t.IsCompleted);
            }
            else
                return 100;

            return (int)Math.Round((double)completedTasks / totalTasks * 100);
        }
    }
}
