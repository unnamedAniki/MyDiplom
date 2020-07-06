using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Kind
    {
        public Kind()
        {
            Type = new HashSet<Type>();
        }

        public int Id { get; set; }
        public string KindName { get; set; }

        public virtual ICollection<Type> Type { get; set; }
    }
}
