using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Staff 
    {
        public Staff()
        {
            Users = new HashSet<Users>();
        }

        public int Id { get; set; }
        public string Staff_Name { get; set; }

        public virtual ICollection<Users> Users { get; set; }
    }
}
