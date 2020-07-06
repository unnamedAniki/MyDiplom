using System;
using System.Collections.Generic;

namespace AccoutingDocs.Models
{
    public partial class Documents
    {
        public Documents()
        {
            Register = new HashSet<Register>();
            Staffdocuments = new HashSet<Staffdocuments>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Commend { get; set; }
        public int Iddocument { get; set; }
        public string Path { get; set; }
        public DateTime AddingDate { get; set; }
        public int NameFrom { get; set; }
        public int StatusId { get; set; }
        public DateTime? UsingDate { get; set; }
        public DateTime? MoveDateToArchieve { get; set; }
        public string UserId { get; set; }
        public string EditUserId { get; set; }

        public virtual Type IddocumentNavigation { get; set; }
        public virtual Organization NameFromNavigation { get; set; }
        public virtual Status Status { get; set; }
        public virtual Users User { get; set; }
        public virtual Users EditUser { get; set; }
        public virtual ICollection<Register> Register { get; set; }
        public virtual ICollection<Staffdocuments> Staffdocuments { get; set; }
        /// <summary>
        /// Поиск данных
        /// </summary>
        /// <param name="name">Параметр поиска</param>
        /// <returns>Найденный результат</returns>
        public bool Search(string name)
        {
            name = name.ToLower();
            return Id.ToString().ToLower().Contains(name);
        }
    }
}
