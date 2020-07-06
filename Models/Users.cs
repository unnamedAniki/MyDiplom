using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Users
    {
        public Users()
        {
            EditDocuments = new HashSet<Documents>();
            Documents = new HashSet<Documents>();
            Register = new HashSet<Register>();
            Staffdocuments = new HashSet<Staffdocuments>();
            Userroles = new HashSet<Userroles>();
        }

        public string Id { get; set; }
        public int StaffId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Fam { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public virtual Staff Staff { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }
        public virtual ICollection<Documents> EditDocuments { get; set; }
        public virtual ICollection<Register> Register { get; set; }
        public virtual ICollection<Staffdocuments> Staffdocuments { get; set; }
        public virtual ICollection<Userroles> Userroles { get; set; }
    }
}
