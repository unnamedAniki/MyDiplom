using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.DIalogContents;
using AccoutingDocs.Models;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AccoutingDocs.ViewModels
{
    public class MainMenuViewModel : INotifyPropertyChanged
    {
        public MainMenuViewModel()
        {
            UserName = AuthViewModel.currentUser;
            Menus = MainMenu.GetMenu(AuthViewModel.Role);
            Panel = MainMenu.GetPanel(AuthViewModel.Role);
            Title = "Панель быстрого доступа";
            Result = "Загрузка писем...";
            Value = 0;
        }
        #region Свойства с работой меню
        public bool exec = false;

        /// <summary>
        /// Команда восстановления данных в БД
        /// </summary>
        public ICommand RecoverData
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Recover(context);
                    }
                });
            }
        }
        /// <summary>
        /// Команда сохранения данных из БД
        /// </summary>
        public ICommand SaveAll
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        if (!Directory.Exists(Environment.CurrentDirectory + @"\Сохраненные данные"))
                            Directory.CreateDirectory(Environment.CurrentDirectory + @"\Сохраненные данные");
                        SavingData(context);
                    }
                });
            }
        }
        /// <summary>
        /// Команда открытия окна для создания нового подключения
        /// </summary>
        public ICommand OpenNewConnectionCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new NewConnectionContentt(), "ShowHelp");
                });
            }
        }
        /// <summary>
        /// Команда создания нового подключения
        /// </summary>
        public ICommand NewConnectionCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    MessageBox.Show("Подключение обновлено!\nЧтобы изменения вступил в силу, программа перезагрузится", "Успешно");
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    Connection temp = new Connection
                    {
                        Server = Server,
                        Database = Database,
                        Username = Username,
                        Password = Password
                    };
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    if (File.Exists(Environment.CurrentDirectory + @"\Строка подключения.txt"))
                        File.Delete(Environment.CurrentDirectory + @"\Строка подключения.txt");
                    using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Строка подключения.txt"))
                    using (JsonWriter writer = new JsonTextWriter(sw))
                    {
                        serializer.Serialize(writer, temp);
                    }
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName);
                    Application.Current.Shutdown();
                });
            }
        }
        /// <summary>
        /// Выход из приложения
        /// </summary>
        public ICommand ExitApp
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Environment.Exit(0);
                });
            }
        }
        /// <summary>
        /// Выход на форму авторизации
        /// </summary>
        public ICommand ExitToAuth
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var currentwin = (HeadMenu)obj;
                    exec = true;
                    currentwin.Close();
                    App.main.Show();
                });
            }
        }
        /// <summary>
        /// Справочная информация
        /// </summary>
        public ICommand Help
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new DIalogContents.SupportView(), "ShowHelp");
                });
            }
        }
        private string _Title { get; set; }
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }
        private string _Server { get; set; }
        public string Server
        {
            get
            {
                return _Server;
            }
            set
            {
                _Server = value;
                OnPropertyChanged("Server");
            }
        }
        private string _Database { get; set; }
        public string Database
        {
            get
            {
                return _Database;
            }
            set
            {
                _Database = value;
                OnPropertyChanged("Database");
            }
        }
        private string _Username { get; set; }
        public string Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
                OnPropertyChanged("UserName");
            }
        }
        private string _Password { get; set; }
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }

        private int _Value { get; set; }
        public int Value
        {
            get
            {
                return _Value;
            }
            set
            {
                _Value = value;
                OnPropertyChanged("Value");
            }
        }
        private string _Result { get; set; }
        public string Result
        {
            get
            {
                return _Result;
            }
            set
            {
                _Result = value;
                OnPropertyChanged("Result");
            }
        }
        private MainMenu _SelectedPanel;
        public MainMenu SelectedPanel
        {
            get
            {
                return _SelectedPanel;
            }
            set
            {
                if (SelectedMenu != null)
                    SelectedMenu = null;
                _SelectedPanel = value;
                OnPropertyChanged("SelectedPanel");
            }
        }
        private ObservableCollection<MainMenu> _Panel;
        public ObservableCollection<MainMenu> Panel
        {
            get
            {
                return _Panel;
            }
            set
            {
                _Panel = value;
                OnPropertyChanged("Panel");
            }
        }
        private MainMenu _SelectedMenu;
        public MainMenu SelectedMenu
        {
            get
            {
                return _SelectedMenu;
            }
            set
            {
                if (SelectedPanel != null)
                    SelectedPanel = null;
                _SelectedMenu = value;
                Title = _SelectedMenu.Title;
                DrawerHost.CloseDrawerCommand.Execute(Dock.Left, null);
                OnPropertyChanged("SelectedMenu");
            }
        }
        private ObservableCollection<MainMenu> _Menus;
        public ObservableCollection<MainMenu> Menus
        {
            get
            {
                return _Menus;
            }
            set
            {
                _Menus = value;
                OnPropertyChanged("Menus");
            }
        }
        private Userroles _UserName { get; set; }
        public Userroles UserName
        {
            get
            {
                return _UserName;
            }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
            }
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Сохраняет данные базы данных в json формате
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
        private void SavingData(documentContext context)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Подразделения.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Staff);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Пользователи.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Users);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Роли.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Roles);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Статусы.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Status);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Виды документов.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Kind);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Организации.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Organization);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Типы документов.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Type);
            }
            using (StreamWriter sw = new StreamWriter(Environment.CurrentDirectory + @"\Сохраненные данные\Роли пользователей.txt"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, context.Userroles);
            }
            MessageBox.Show("Данные успешно сохранены!");
        }
        /// <summary>
        /// Восстанавливает данные из созданных json файлов
        /// </summary>
        /// <param name="context">Контекст базы данных</param>
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
            MessageBox.Show("Данные успешно восстановлены!\nЧтобы увидеть новые данные - перезайдите в учетную запись");
        }
        private void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
