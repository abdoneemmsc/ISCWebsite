using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class PostImage
    {
        public int PostId { get; set; }
        public string ImageUrl { get; set; }
        public virtual Post Post { get; set; }

    }
}
