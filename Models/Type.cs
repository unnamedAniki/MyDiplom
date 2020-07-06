using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Type
    {
        public Type()
        {
            Documents = new HashSet<Documents>();
        }
        public int DocumentsId { get; set; }
        public int KindId { get; set; }
        public string Number { get; set; }
        public DateTime DateOfExpire { get; set; }

        public virtual Kind Kind { get; set; }
        public virtual ICollection<Documents> Documents { get; set; }

    }
}
