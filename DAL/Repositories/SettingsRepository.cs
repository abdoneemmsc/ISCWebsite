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
    public class SettingsRepository : Repository<Setting>, ISettingsRepository
    {
        public SettingsRepository(DbContext context) : base(context)
        {
        }
        public Setting Get()
        {
            return _appContext.Settings.OrderByDescending(x => x.Id)?.FirstOrDefault();
        }

        private ApplicationDbContext _appContext => (ApplicationDbContext)_context;
    }
}
