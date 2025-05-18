using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Repositories
{
    public class WorkObjectSectionRepository : EFGenericRepository<WorkObjectSection>, IWorkObjectSectionRepository
    {
        public WorkObjectSectionRepository(AppDbContext dbContext) : base(dbContext)
        {
        }
    }
}
