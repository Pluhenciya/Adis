using Adis.Bll.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Interfaces
{
    public interface IProjectService
    {
        /// <summary>
        /// Добавляет новый проект
        /// </summary>
        /// <param name="project">Новые данные проекта</param>
        /// <returns>Созданный проект</returns>
        public Task<ProjectDto> AddProject(ProjectDto project);
    }
}
