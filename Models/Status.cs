using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Status
    {
        public Status()
        {
            Documents = new HashSet<Documents>();
            Register = new HashSet<Register>();
        }

        public int Id { get; set; }
        public string Status1 { get; set; }

        public virtual ICollection<Documents> Documents { get; set; }
        public virtual ICollection<Register> Register { get; set; }
    }
}
