using Adis.Dm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adis.Dal.Specifications
{
    public class CommentWithUserByIdSpecification : Specification<Comment>
    {
        public CommentWithUserByIdSpecification(int id) 
        {
            ApplyCriteria(c => c.IdComment == id);
            AddInclude(c => c.Sender);
        }
    }
}
