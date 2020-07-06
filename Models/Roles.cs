using System;
using System.Collections.Generic;
using System.Linq;

namespace AccoutingDocs.Models
{
    public partial class Roles
    {
        public Roles()
        {
            Userroles = new HashSet<Userroles>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Access { get; set; }

        public virtual ICollection<Userroles> Userroles { get; set; }
    }
}
