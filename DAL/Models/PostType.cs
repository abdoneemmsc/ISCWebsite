using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class PostType
    {
        public PostType()
        {
            Posts = new HashSet<Post>();
        }
        public byte Id { get; set; }
        public string Name { get; set; }
        public string NameEn { get; set; }
        public ICollection<Post> Posts { get; set; }
        public string Description { get; set; }
        public string DescriptionEn { get; set; }
    }
}
