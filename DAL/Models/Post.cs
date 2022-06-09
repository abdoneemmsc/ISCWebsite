// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Post : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TitleEn { get; set; }
        public string MainImageUrl { get; set; }
        public string IconUrl { get; set; }
        public string Body { get; set; }
        public string BodyEn { get; set; }
        public byte TypeId { get; set; }
        public PostType PostType { get; set; }
        public ICollection<PostImage> PostImages { get; set; }
        public int Hits { get; set; } = 0;
        public bool Published { get; set; } = false;

    }
}
