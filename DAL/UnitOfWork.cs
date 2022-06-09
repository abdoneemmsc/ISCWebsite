// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Repositories;
using DAL.Repositories.Interfaces;

namespace DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly ApplicationDbContext _context;
        IPostsRepository _posts;
        IPostTypeRepository _postTypes;
        IMenuRepository _menus;
        IContactsRepository _contacts;
        public ISettingsRepository _settings;


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IPostsRepository Posts
        {
            get
            {
                if (_posts == null)
                    _posts = new PostsRepository(_context);

                return _posts;
            }
        }

        public IPostTypeRepository PostTypes
        {
            get
            {
                if (_postTypes == null)
                    _postTypes = new PostTypeRepository(_context);

                return _postTypes;
            }
        }

        public IMenuRepository Menus
        {
            get
            {
                if (_menus == null)
                    _menus = new MenuRepository(_context);

                return _menus;
            }
        }

        public IContactsRepository Contacts
        {
            get
            {
                if (_contacts == null)
                    _contacts = new ContactsRepository(_context);

                return _contacts;
            }
        }

        public ISettingsRepository Settings
        {
            get
            {
                if (_settings == null)
                    _settings = new SettingsRepository(_context);

                return _settings;
            }
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }
    }
}
