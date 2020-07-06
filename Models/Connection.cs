using System;
using System.Collections.Generic;
using System.Text;

namespace AccoutingDocs.Models
{
    public class Connection
    {
        public Connection()
        {
            
        }
        public string Server { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Database { get; set; }
        /// <summary>
        /// Создание строки подключения
        /// </summary>
        /// <returns>Возвращает строку подключения</returns>
        public static string Connect(Connection temp)
        {
            return "server=" + temp.Server + ";user id=" + temp.Username + ";password=" + temp.Password + ";persistsecurityinfo=True;database=" + temp.Database;
        }
    }
}
