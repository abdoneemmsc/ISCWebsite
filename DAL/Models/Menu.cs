using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class Menu
    {
        public Menu()
        {
            Children = new HashSet<Menu>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public int? ParentId { get; set; }
        public string Url { get; set; }
        public virtual Menu Parent { get; set; }
        public int? Order { get; set; }
        public bool IsExtended { get; set; } = false;
        public virtual ICollection<Menu> Children { get; set; }
    }
}
