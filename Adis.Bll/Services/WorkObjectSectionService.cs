using Adis.Bll.Interfaces;
using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Bll.Services
{
    public class WorkObjectSectionService : IWorkObjectSectionService
    {
        private readonly IWorkObjectSectionRepository _workObjectSectionRepository;

        public WorkObjectSectionService(IWorkObjectSectionRepository workObjectSectionRepository) 
        { 
            _workObjectSectionRepository = workObjectSectionRepository;
        }

        public async Task<WorkObjectSection> AddWorkObjectSectionAsync(WorkObjectSection workObjectSection)
        {
            return await _workObjectSectionRepository.AddAsync(workObjectSection);
        }
    }
}
