using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    /// <summary>
    /// Позволяет управлять данными проектов
    /// </summary>
    public interface IProjectRepository : IRepository<Project>
    {
    }
}
