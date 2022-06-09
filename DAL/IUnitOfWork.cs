// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public interface IUnitOfWork
    {
        IPostsRepository Posts { get; }
        IPostTypeRepository PostTypes { get; }
        IMenuRepository Menus { get; }
        IContactsRepository Contacts { get; }
        ISettingsRepository Settings { get; }
        int SaveChanges();
    }
}
