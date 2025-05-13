using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Interfaces
{
    public interface ICommentRepository : IRepository<Comment>
    {
        public Task<Comment?> GetCommentByIdAsync(int id);
    }
}
