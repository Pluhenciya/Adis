using Adis.Bll.Dtos;
using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IMapper _mapper;
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IMapper mapper, IProjectRepository projectRepository) 
        { 
            _mapper = mapper;
            _projectRepository = projectRepository;
        }

        public async Task<ProjectDto> AddProject(ProjectDto project)
        {
            return _mapper.Map<ProjectDto>(await _projectRepository.AddAsync(_mapper.Map<Project>(project)));
        }
    }
}
