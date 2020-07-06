using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AccoutingDocs.Models;
using AccoutingDocs.Supports;
using AccoutingDocs.ViewModels;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccoutingDocs
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Window window = null;
        public static Window main = null;
        public App()
        {
            if (CheckConnect())
            {
                window = new Window();
            }
            else
            {
                if (MessageBox.Show("Произошла ошибка при подключении к базе данных! Проверьте в папке программы файл 'Строка подключения.txt'!", "Ошибка!", MessageBoxButton.OK) == MessageBoxResult.OK)
                {
                    App.Current.Shutdown();
                }
            }
        }
        /// <summary>
        /// Выход из приложения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitApp(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }
        /// <summary>
        /// Скрывает окно
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Minimazed(object sender, RoutedEventArgs e)
        {
            window.WindowState = WindowState.Minimized;
        }
        public static Dispatcher MainMenuDispatcher { get; set; }
        public static MainMenuViewModel model { get; set; }
        /// <summary>
        /// Проверка соединения
        /// </summary>
        /// <returns>Возвращает true или false</returns>
        private bool CheckConnect()
        {
            try
            {
                using (Models.documentContext context = new Models.documentContext())
                {
                    Recover(context);
                    var user = context.Users.Where(p => p.UserName == "admin").FirstOrDefault();
                    var role = context.Roles.Where(p => p.Name == "Администратор").FirstOrDefault();
                    var staff = context.Staff.Where(p => p.Staff_Name == "ИТ и ТО").FirstOrDefault();
                    if(staff == null)
                    {
                        Staff temp = new Staff
                        {
                            Staff_Name = "ИТ и ТО"
                        };
                        context.Staff.Add(temp);
                        context.SaveChanges();
                    }
                    if (user == null)
                    {
                        Models.Users users = new Models.Users
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = "admin",
                            Email = "admin@root.com",
                            Fam = "Иванов",
                            Name = "Иван",
                            Lastname = "Иванович",
                            PasswordHash = PasswordHasher.HashPassword("123"),
                            StaffId = context.Staff.FirstOrDefault(p => p.Staff_Name == "ИТ и ТО").Id
                        };
                        context.Users.Add(users);
                        context.SaveChanges();
                    }
                    if (role == null)
                    {
                        Models.Roles roles = new Models.Roles
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = "Администратор",
                            Access = "0"
                        };
                        context.Roles.Add(roles);
                        context.SaveChanges();
                    }
                    var result = context.Userroles.Include(p => p.User).Include(p => p.Role).FirstOrDefault();
                    if (result == null || result.Role.Name != "Администратор")
                    {
                        Models.Userroles userroles = new Models.Userroles
                        {
                            User = context.Users.Where(p => p.UserName == "admin").FirstOrDefault(),
                            Role = context.Roles.Where(p => p.Name == "Администратор").FirstOrDefault(),
                        };
                        context.Userroles.Add(userroles);
                        context.SaveChanges();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void Recover(documentContext context)
        {
            var serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Подразделения.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Подразделения.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Staff>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Staff.Any(p => p.Id == item.Id))
                        {
                            context.Staff.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Статусы.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Статусы.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Status>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Status.Any(p => p.Id == item.Id))
                        {
                            context.Status.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Роли.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Роли.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Roles>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Roles.Any(p => p.Id == item.Id))
                        {
                            context.Roles.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Организации.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Организации.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Organization>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Organization.Any(p => p.IdOrganization == item.IdOrganization))
                        {
                            context.Organization.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Виды документов.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Виды документов.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Kind>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Kind.Any(p => p.Id == item.Id))
                        {
                            context.Kind.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Типы документов.txt"))
            {

                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Типы документов.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Models.Type>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Type.Any(p => p.DocumentsId == item.DocumentsId))
                        {
                            item.Kind = null;
                            context.Type.Add(item);
                            context.SaveChanges();
                        }
                    }
                }

            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Пользователи.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Пользователи.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Users>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Users.Any(p => p.Id == item.Id))
                        {
                            item.Staff = null;
                            context.Users.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
            if (File.Exists(Environment.CurrentDirectory + @"\Сохраненные данные\Роли Пользователей.txt"))
            {
                using (StreamReader sw = new StreamReader(Environment.CurrentDirectory + @"\Сохраненные данные\Роли Пользователей.txt"))
                using (JsonReader writer = new JsonTextReader(sw))
                {
                    var temp = serializer.Deserialize<List<Userroles>>(writer);
                    foreach (var item in temp)
                    {
                        if (!context.Userroles.Any(p => p.UserId == item.UserId))
                        {
                            item.User = null;
                            item.Role = null;
                            context.Userroles.Add(item);
                            context.SaveChanges();
                        }
                    }
                }
            }
        }
    }
}
