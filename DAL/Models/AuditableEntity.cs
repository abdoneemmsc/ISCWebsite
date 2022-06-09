// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using DAL.Models.Interfaces;

namespace DAL.Models
{
    public class AuditableEntity : IAuditableEntity
    {
        [MaxLength(450)]
        public string CreatedById { get; set; }
        [MaxLength(450)]
        public string UpdatedById { get; set; }
        public DateTime UpdatedDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public ApplicationUser UpdatedBy { get; set; }

    }
}
