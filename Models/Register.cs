using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.Models
{
    public partial class Register
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public DateTime TakenDate { get; set; }
        public DateTime ReturningDate { get; set; }
        public string UserId { get; set; }
        public int StatusId { get; set; }

        public virtual Documents Document { get; set; }
        public virtual Users User { get; set; }
        public virtual Status Status { get; set; }
        /// <summary>
        /// Добавление данных
        /// </summary>
        public void Add(Users headuser, Documents doc)
        {
            using (documentContext context = new documentContext())
            {
                if (!context.Register.Where(p => p.Document.Name == doc.Name && p.StatusId == doc.StatusId && p.TakenDate == DateTime.Now).Any())
                {
                    Register temp = new Register
                    {
                        TakenDate = DateTime.Now,
                        DocumentId = doc.Id,
                        UserId = headuser.Id,
                        ReturningDate = DateTime.Now.Add(TimeSpan.FromDays(7)),
                        StatusId = doc.StatusId
                    };
                    context.Register.Add(temp);
                    context.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Добавление данных при использованнии документов
        /// </summary>
        public void AddFromUsing(Users headuser, Documents doc)
        {
            using (documentContext context = new documentContext())
            {
                if (!context.Register.Where(p => p.Document.Name == doc.Name && p.StatusId == doc.StatusId && p.TakenDate == DateTime.Now).Any())
                {
                    Register temp = new Register
                    {
                        TakenDate = doc.UsingDate.Value,
                        DocumentId = doc.Id,
                        UserId = headuser.Id,
                        ReturningDate = DateTime.Now,
                        StatusId = doc.StatusId
                    };
                    context.Register.Add(temp);
                    context.SaveChanges();
                }
            }
        }
        /// <summary>
        /// Поиск данных
        /// </summary>
        /// <param name="name">Параметр поиска</param>
        /// <returns>Найденный результат</returns>
        public bool Search(string name)
        {
            name = name.ToLower();
            return Document.Commend.ToLower().Contains(name);
        }
    }
}
