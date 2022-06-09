// =============================
// Email: abdoneem@gmail.com
// 
// =============================
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IPostsRepository : IRepository<Post>
    {
        Post GetPostWithChilds(int id);
        IEnumerable<Post> GetTop(Func<Post, bool> p, int Limit = 10);
        IEnumerable<Post> FindWithPostType(Func<Post, bool> p);
        IEnumerable<Post> GetAllWithChilds();
    }
}
