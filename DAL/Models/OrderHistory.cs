using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models
{
    public class OrderHistory : AuditableEntity
    {
        public int Id { get; set; }

        public byte StatusId { get; set; }
        public int OrderId { get; set; }
        public string Notes { get; set; }

        public Post Order { get; set; }
        public PostType OrderStatusType { get; set; }

    }
}
