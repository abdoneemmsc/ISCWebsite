// =============================
// Email: abdoneem@gmail.com
// 
// =============================

using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Models.Interfaces
{
    public interface IAuditableEntity
    {
        string CreatedById { get; set; }
        string UpdatedById { get; set; }
        DateTime CreatedDate { get; set; }
        DateTime UpdatedDate { get; set; }

        public ApplicationUser CreatedBy { get; set; }
        public ApplicationUser UpdatedBy { get; set; }
    }
}
