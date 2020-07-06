using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Organization
    {
        public Organization()
        {
            Documents = new HashSet<Documents>();
        }

        public int IdOrganization { get; set; }
        public string Mail { get; set; }
        public string Name { get; set; }
        public string HeadName { get; set; }

        public virtual ICollection<Documents> Documents { get; set; }
    }
}
