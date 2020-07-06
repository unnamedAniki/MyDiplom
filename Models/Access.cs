using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AccoutingDocs.Models
{
    public class Access
    {
        public Access()
        {
        }
        public int AccessID { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// Получить список уровней доступа
        /// </summary>
        /// <returns>Коллекицю уровней</returns>
        public static ObservableCollection<Access> GetAccesses()
        {
            var temp = new ObservableCollection<Access>();
            temp.Add(new Access { AccessID = 1, Name = "Полный доступ" });
            temp.Add(new Access { AccessID = 1, Name = "Доступ к документам" });
            temp.Add(new Access { AccessID = 2, Name = "Частичный доступ" });
            temp.Add(new Access { AccessID = 3, Name = "Доступ к справочникам" });
            temp.Add(new Access { AccessID = 4, Name = "Только личный кабинет" });
            return temp;
        }
    }
}
