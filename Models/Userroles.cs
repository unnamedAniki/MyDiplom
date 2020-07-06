using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Userroles
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public virtual Roles Role { get; set; }
        public virtual Users User { get; set; }
    }
}
