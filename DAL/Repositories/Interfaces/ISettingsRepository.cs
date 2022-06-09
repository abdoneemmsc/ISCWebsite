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
    public interface ISettingsRepository : IRepository<Setting>
    {
        public Setting Get();
    }
}
