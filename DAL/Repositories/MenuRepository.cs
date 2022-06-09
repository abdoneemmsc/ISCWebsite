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
    public class MenuRepository : Repository<Menu>, IMenuRepository
    {
        public MenuRepository(DbContext context) : base(context)
        {
        }



        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;

        public Menu GeMenuWithChilds(int id)
        {
            return _appContext.Menus.Include(a => a.Children).FirstOrDefault(a => a.Id == id);
        }

        public IEnumerable<Menu> GetAllWithChilds()
        {
            return _appContext.Menus.Include(a => a.Children).OrderBy(a => a.Order).AsEnumerable();
        }
    }
}
