using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using AccoutingDocs.Supports;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        public RegisterViewModel()
        {
            GetRegisterData();
            GetSendingDocData();
            GetUsingDocData();
            GetUsersData();
            GetTypeData();
            GetStatusData();
            GetOrganizationData();
        }
        #region Команды ViewModel
        /// <summary>
        /// Команда поиска
        /// </summary>
        public ICommand Search
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SearchText != "")
                    {
                        GetSearchResult(SearchText);
                        SearchText = "";
                    }
                    else
                    {
                        GetRegisterData();
                    }
                });
            }
        }
        /// <summary>
        /// Команда обновления
        /// </summary>
        public ICommand Refresh
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GetRegisterData();
                    GetSendingDocData();
                    GetUsingDocData();
                });
            }
        }
        #endregion
        #region Свойства модели
        #region Публичные свойства
        public ObservableCollection<Register> Register
        {
            get { return _Register; }
            set
            {
                _Register = value;
                OnPropertyChanged("Register");
            }
        }
        private ObservableCollection<Models.Type> _Types { get; set; }
        public ObservableCollection<Models.Type> Types
        {
            get { return _Types; }
            set
            {
                _Types = value;
                OnPropertyChanged("Types");
            }
        }
        private ObservableCollection<Users> _Users { get; set; }
        public ObservableCollection<Users> Users
        {
            get { return _Users; }
            set
            {
                _Users = value;
                OnPropertyChanged("Users");
            }
        }
        private ObservableCollection<Organization> _Organizations { get; set; }
        public ObservableCollection<Organization> Organizations
        {
            get { return _Organizations; }
            set
            {
                _Organizations = value;
                OnPropertyChanged("Organizations");
            }
        }
        private ObservableCollection<Status> _Statuses { get; set; }
        public ObservableCollection<Status> Statuses
        {
            get { return _Statuses; }
            set
            {
                _Statuses = value;
                OnPropertyChanged("Statuses");
            }
        }
        private string _Count { get; set; }
        public string Count
        {
            get { return _Count; }
            set
            {
                _Count = value;
                OnPropertyChanged("Count");
            }
        }
        private string _ArchCount { get; set; }
        public string ArchCount
        {
            get { return _ArchCount; }
            set
            {
                _ArchCount = value;
                OnPropertyChanged("ArchCount");
            }
        }
        private string _SearchText { get; set; }
        public string SearchText
        {
            get { return _SearchText; }
            set
            {
                _SearchText = value;
                OnPropertyChanged("SearchText");
            }
        }
        private string _UsingCount { get; set; }
        public string UsingCount
        {
            get { return _UsingCount; }
            set
            {
                _UsingCount = value;
                OnPropertyChanged("UsingCount");
            }
        }
        private string _InArchieveCount { get; set; }
        public string InArchieveCount
        {
            get { return _InArchieveCount; }
            set
            {
                _InArchieveCount = value;
                OnPropertyChanged("InArchieveCount");
            }
        }
        #endregion
        #region Приватные свойства
        private int _Id { get; set; }
        private string _Name { get; set; }
        private int _Iddocument { get; set; }
        private string _Path { get; set; }
        private DateTime _AddingDate { get; set; }
        private int _NameFrom { get; set; }
        private int _StatusId { get; set; }
        private DateTime? _UsingDate { get; set; }
        private DateTime? _MoveDateToArchieve { get; set; }
        private DateTime? _DateToDelete { get; set; }
        private string _UserId { get; set; }
        private Models.Type _IddocumentNavigation { get; set; }
        private Organization _NameFromNavigation { get; set; }
        private Status _Status { get; set; }
        private Users _User { get; set; }
        private Register selectedRegister { get; set; }
        private MainMenu _SelectedMenu;
        private ObservableCollection<Register> _Register { get; set; }
        private ObservableCollection<Register> _SendingRegister { get; set; }
        private ObservableCollection<Register> _UsingRegister { get; set; }
        public ObservableCollection<Register> SendingRegister
        {
            get { return _SendingRegister; }
            set
            {
                _SendingRegister = value;
                OnPropertyChanged("SendingRegister");
            }
        }
        public ObservableCollection<Register> UsingRegister
        {
            get { return _UsingRegister; }
            set
            {
                _UsingRegister = value;
                OnPropertyChanged("UsingRegister");
            }
        }
        #endregion
        #region Свойства полей ViewModel
        public int Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public int Iddocument
        {
            get { return _Iddocument; }
            set
            {
                _Iddocument = value;
                OnPropertyChanged("Iddocument");
            }
        }
        public string Path
        {
            get { return _Path; }
            set
            {
                _Path = value;
                OnPropertyChanged("Path");
            }
        }
        public DateTime AddingDate
        {
            get { return _AddingDate; }
            set
            {
                _AddingDate = value;
                OnPropertyChanged("AddingDate");
            }
        }
        public int NameFrom
        {
            get { return _NameFrom; }
            set
            {
                _NameFrom = value;
                OnPropertyChanged("NameFrom");
            }
        }
        public int StatusId
        {
            get { return _StatusId; }
            set
            {
                _StatusId = value;
                OnPropertyChanged("StatusId");
            }
        }
        public DateTime? UsingDate
        {
            get { return _UsingDate; }
            set
            {
                _UsingDate = value;
                OnPropertyChanged("UsingDate");
            }
        }
        public DateTime? MoveDateToArchieve
        {
            get { return _MoveDateToArchieve; }
            set
            {
                _MoveDateToArchieve = value;
                OnPropertyChanged("MoveDateToArchieve");
            }
        }
        public DateTime? DateToDelete
        {
            get { return _DateToDelete; }
            set
            {
                _DateToDelete = value;
                OnPropertyChanged("DateToDelete");
            }
        }
        public string UserId
        {
            get { return _UserId; }
            set
            {
                _UserId = value;
                OnPropertyChanged("UserId");
            }
        }
        private DateTime _EndingDate { get; set; }
        public MainMenu SelectedMenu
        {
            get
            {
                return _SelectedMenu;
            }
            set
            {
                _SelectedMenu = value;
                OnPropertyChanged("SelectedMenu");
            }
        }
        #endregion
        #region Свойства моделей ViewModel
        public Models.Type IddocumentNavigation
        {
            get { return _IddocumentNavigation; }
            set
            {
                _IddocumentNavigation = value;
                OnPropertyChanged("IddocumentNavigation");
            }
        }
        public Organization NameFromNavigation
        {
            get { return _NameFromNavigation; }
            set
            {
                _NameFromNavigation = value;
                OnPropertyChanged("NameFromNavigation");
            }
        }
        public Status Status
        {
            get { return _Status; }
            set
            {
                _Status = value;
                OnPropertyChanged("Status");
            }
        }
        public Users User
        {
            get { return _User; }
            set
            {
                _User = value;
                OnPropertyChanged("User");
            }
        }
        public Register SelectedRegister
        {
            get { return selectedRegister; }
            set
            {
                selectedRegister = value;
                OnPropertyChanged("SelectedRegister");
            }
        }
        #endregion
        #endregion
        #region Методы модели
        /// <summary>
        /// Загружает в коллекцию данные об входящих документах организации
        /// </summary>
        private void GetRegisterData()
        {
            Register = new ObservableCollection<Register>();
            int ArchiveCount = 0;
            int usingCount = 0;
            using (documentContext context = new documentContext())
            {
                List<Register> temp = context.Register
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Status)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Where(p => p.Status.Status1 == "Проверен" || p.Status.Status1 == "В архиве")
                    .ToList();
                foreach (var item in temp)
                {
                    if (item.StatusId == context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "В архиве").Id)
                    {
                        ArchiveCount++;
                    }
                    if (item.StatusId == context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Используется").Id)
                    {
                        usingCount++;
                    }
                    Register.Add(item);
                }
                Count = "Количество входящих документов: " + (temp.Count - ArchiveCount).ToString();
                ArchCount = "Количество архивных документов: " + ArchiveCount.ToString();
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные об внутренних документах организации
        /// </summary>
        private void GetUsingDocData()
        {
            UsingRegister = new ObservableCollection<Register>();
            using (documentContext context = new documentContext())
            {
                List<Register> temp = context.Register
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Status)
                    .Include(p => p.Document)
                    .ThenInclude(p=>p.NameFromNavigation)
                    .Where(p => p.Status.Status1 == "Используется")
                    .ToList();
                foreach (var item in temp)
                {
                    UsingRegister.Add(item);
                }
                UsingCount = "Количество использований документов: " + temp.Count.ToString();
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные об исходящих документах организации
        /// </summary>
        private void GetSendingDocData()
        {
            SendingRegister = new ObservableCollection<Register>();
            using (documentContext context = new documentContext())
            {
                List<Register> temp = context.Register
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Status)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.EditUser)
                    .ThenInclude(p => p.Staff)
                    .Where(p => p.Status.Status1 == "Отправлен")
                    .ToList();
                foreach (var item in temp)
                {
                    SendingRegister.Add(item);
                }
                InArchieveCount = "Количество исходящих документов: " + temp.Count.ToString();
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Type, включая данные по внешним ключам
        /// </summary>
        private void GetTypeData()
        {
            Types = new ObservableCollection<Models.Type>();
            using (documentContext context = new documentContext())
            {
                List<Models.Type> temp = context.Type.Include(p => p.Kind).ToList();
                foreach (var item in temp)
                {
                    Types.Add(item);
                }
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Status, включая данные по внешним ключам
        /// </summary>
        private void GetStatusData()
        {
            Statuses = new ObservableCollection<Status>();
            using (documentContext context = new documentContext())
            {
                List<Status> temp = context.Status.AsNoTracking().ToList();
                foreach (var item in temp)
                {
                    Statuses.Add(item);
                }
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Organization, включая данные по внешним ключам
        /// </summary>
        private void GetOrganizationData()
        {
            Organizations = new ObservableCollection<Organization>();
            using (documentContext context = new documentContext())
            {
                List<Organization> temp = context.Organization.ToList();
                foreach (var item in temp)
                {
                    Organizations.Add(item);
                }
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Users, включая данные по внешним ключам
        /// </summary>
        private void GetUsersData()
        {
            Users = new ObservableCollection<Users>();
            using (documentContext context = new documentContext())
            {
                List<Users> temp = context.Users.Include(p => p.Staff).ToList();
                foreach (var item in temp)
                {
                    Users.Add(item);
                }
            }
        }
        /// <summary>
        /// Получить результат фильтрации
        /// </summary>
        /// <param name="text">Параметр фильтра</param>
        private void GetSearchResult(string text)
        {
            var temp = Register.Where(p => p.Document.NameFromNavigation.Name == text || p.Document.Commend == text).ToList();
            var usingTemp = UsingRegister.Where(p => p.Document.NameFromNavigation.Name == text || p.Document.Commend == text).ToList();
            var sendingTemp = SendingRegister.Where(p => p.Document.NameFromNavigation.Name == text || p.Document.Commend == text).ToList();
            Register = new ObservableCollection<Register>();
            foreach (var item in temp)
            {
                Register.Add(item);
            }
            UsingRegister = new ObservableCollection<Register>();
            foreach (var item in usingTemp)
            {
                UsingRegister.Add(item);
            }
            SendingRegister = new ObservableCollection<Register>();
            foreach (var item in sendingTemp)
            {
                SendingRegister.Add(item);
            }
        }
        #endregion
        #region Событие перехвата изменения свойства
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        #endregion
    }
}
