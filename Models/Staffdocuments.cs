using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.Models
{
    public partial class Staffdocuments
    {
        public int Id { get; set; }
        public string HeadUserId { get; set; }
        public int DocumentId { get; set; }
        public string Comments { get; set; }
        public DateTime EndingDate { get; set; }

        public virtual Documents Document { get; set; }
        public virtual Users HeadUser { get; set; }
        /// <summary>
        /// Добавление данных
        /// </summary>
        public void Add(Users headuser, Documents doc, string info)
        {
            using (documentContext context = new documentContext())
            {
                if (!context.Staffdocuments.Where(p => p.Document.Name == doc.Name).Any())
                {
                    Staffdocuments temp = new Staffdocuments
                    {
                        EndingDate = DateTime.Now.Add(TimeSpan.FromDays(7)),
                        DocumentId = doc.Id,
                        HeadUserId = headuser.Id,
                        Comments = info
                    };
                    context.Staffdocuments.Add(temp);
                    context.SaveChanges();
                }
            }
        }
    }
}
