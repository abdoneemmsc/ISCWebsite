// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DAL.Repositories.Interfaces;

namespace DAL.Repositories
{
    public class PostsRepository : Repository<Post>, IPostsRepository
    {
        public PostsRepository(DbContext context) : base(context)
        {
        }



        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public IEnumerable<Post> FindWithPostType(Func<Post, bool> p)
        {
            return this._appContext.Posts.Include(a => a.PostType).Where(p).OrderByDescending(x=>x.Id);
        }

        public IEnumerable<Post> GetAllWithChilds()
        {
            return _appContext.Posts.Include(a => a.CreatedBy).Include(a => a.PostType);
        }

        public Post GetPostWithChilds(int id)
        {
            return this._appContext.Posts.Include(a => a.PostType).Include(a => a.PostImages).FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Post> GetTop(Func<Post, bool> p, int Limit = 10)
        {
            return this._appContext.Posts.Include(a => a.PostType).Where(p).OrderByDescending(a => a.Id).Take(Limit);
        }
    }
}
