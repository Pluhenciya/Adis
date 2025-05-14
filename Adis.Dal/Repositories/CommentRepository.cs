using Adis.Dal.Data;
using Adis.Dal.Interfaces;
using Adis.Dal.Specifications;
using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Repositories
{
    public class CommentRepository : EFGenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(AppDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Comment?> GetCommentByIdAsync(int id)
        {
            CommentWithUserByIdSpecification spec = new(id);
            return (await GetAsync(spec)).FirstOrDefault();
        }
    }
}
