using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using AccoutingDocs.Supports;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.IO;
using AccoutingDocs.DIalogContents;

namespace AccoutingDocs.ViewModels
{
    public class DocumentViewModel : INotifyPropertyChanged
    {
        public DocumentViewModel()
        {
            dialogService = new DialogService();
            EndingDate = DateTime.Now;
            CreateDate = DateTime.Now;
            Name = "";
            TypeNumber = "";
            Comment = "";
            CheckDates();
            GetDocumentData();
            GetUsersData();
            GetTypeData();
            GetStatusData();
            GetOrganizationData();
        }
        #region Команды ViewModel
        private DialogService dialogService;
        /// <summary>
        /// Команда открытия диалога
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    ClearFields();
                    SelectedDocuments = null;
                    GetTypeData();
                    GetStatusData();
                    GetOrganizationData();
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new UserDialogContent(), "Docs");
                });
            }
        }
        /// <summary>
        /// Команда открытия диалога редактирования 
        /// </summary>
        public ICommand OpenEditCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    Name = SelectedDocuments.Name;
                    Path = SelectedDocuments.Path;
                    IddocumentNavigation = SelectedDocuments.IddocumentNavigation.Kind;
                    NameFromNavigation = SelectedDocuments.NameFromNavigation;
                    TypeNumber = SelectedDocuments.IddocumentNavigation.Number;
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new UserDialogContent(), "Docs");
                }, (obj) => SelectedDocuments != null);
            }
        }
        /// <summary>
        /// Команда открытия диалога на отправление выбранного документа
        /// </summary>
        public ICommand OpenSendingCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new SendSelectedDoc(), "Docs");
                }, (obj) => SelectedDocuments != null);
            }
        }
        /// <summary>
        /// Команда открытия диалога на отправление выбранного документа
        /// </summary>
        public ICommand OpenToCreateSendingCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    await DialogHost.Show(new SendControl(), "Docs");
                }, (obj) => SelectedDocuments != null);
            }
        }
        /// <summary>
        /// Команда открытия диалога на использованние выбранного документа
        /// </summary>
        public ICommand OpenUsingCommand
        {
            get
            {
                return new DelegateCommand(async obj =>
                {
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    Comment = "";
                    await DialogHost.Show(new UsingSelectedDoc(), "Docs");
                }, (obj) => SelectedDocuments != null &&
                SelectedDocuments.Status.Status1 != "В архиве" &&
                SelectedDocuments.Status.Status1 != "Отправлен" &&
                SelectedDocuments.Status.Status1 != "Используется" &&
                GetAccess() <= 2);
            }
        }
        /// <summary>
        /// Команда получения имени файла
        /// </summary>
        public ICommand GetFileName
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    dialogService = new DialogService();
                    Name = dialogService.GetFileName();
                });
            }
        }
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
                        GetDocumentData();
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
                    CheckDates();
                    GetDocumentData();
                    GetUsersData();
                    GetTypeData();
                    GetStatusData();
                    GetOrganizationData();
                });
            }
        }
        /// <summary>
        /// Команда получения пути файла
        /// </summary>
        public ICommand GetFilePath
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Path = dialogService.GetFilePath();
                });
            }
        }
        /// <summary>
        /// Команда добавления документа
        /// </summary>
        public ICommand AddDocument
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DateTime date = DateTime.Now;
                    SelectedDocuments = null;
                    var Commend = "";
                    if (!String.IsNullOrEmpty(TypeNumber))
                        Commend = TypeNumber;
                    else
                        Commend = "Без номера";
                    using (documentContext context = new documentContext())
                    {
                        var Temp = new Models.Type()
                        {
                            KindId = IddocumentNavigation.Id,
                            Number = Commend,
                            DateOfExpire = date.AddYears(5)
                        };
                        if (!ValidateType())
                        {
                            context.Type.Add(Temp);
                            context.SaveChanges();
                        }
                        Documents document = new Documents()
                        {
                            Name = IddocumentNavigation.KindName + " " + Name,
                            Commend = Commend + " от " + CreateDate.ToShortDateString(),
                            NameFromNavigation = context.Organization.Find(NameFromNavigation.IdOrganization),
                            IddocumentNavigation = context.Type.Include(p => p.Kind).FirstOrDefault(p => p.Number == Temp.Number && p.KindId == Temp.KindId),
                            AddingDate = date,
                            Path = Path,
                            Status = context.Status.FirstOrDefault(p => p.Status1 == "Проверен"),
                            User = context.Users.Include(p => p.Staff).FirstOrDefault(p => p.Id == AuthViewModel.currentUser.User.Id)
                        };
                        if (!context.Documents.Any(p => (p.Commend == Commend && p.Path == Path)))
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            var temp_ = new Register();
                            context.Documents.Add(document);
                            context.SaveChanges();
                            document = context.Documents.FirstOrDefault(p=>p.Name == document.Name && p.Path == document.Path);
                            temp_.Add(AuthViewModel.currentUser.User, document);
                            GetDocumentData();
                            GetUsersData();
                            GetTypeData();
                            GetStatusData();
                            GetOrganizationData();
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данный документ уже используется");
                        }
                        GetDocumentData();
                        GetUsersData();
                        GetTypeData();
                        GetStatusData();
                        GetOrganizationData();
                        ClearFields();
                    }
                }, (obj) => SelectedDocuments == null && NameFromNavigation != null && IddocumentNavigation != null);
            }
        }
        /// <summary>
        /// Команда добавления документа
        /// </summary>
        public ICommand EditDocument
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DateTime date = DateTime.Now;
                    using (documentContext context = new documentContext())
                    {
                        var Temp = context.Type.Find(SelectedDocuments.IddocumentNavigation.DocumentsId);
                        Temp.KindId = IddocumentNavigation.Id;
                        var Commend = "";
                        if (!String.IsNullOrEmpty(TypeNumber))
                            Commend = TypeNumber;
                        else
                            Commend = "Без номера";
                        Temp.Number = Commend;
                        Temp.DateOfExpire = date.AddYears(5);
                        if (!ValidateType())
                        {
                            context.Type.Update(Temp);
                            context.SaveChanges();
                        }
                        var document = context.Documents.Include(p => p.User).ThenInclude(p => p.Staff)
                                    .Include(p => p.IddocumentNavigation)
                                    .ThenInclude(p => p.Kind)
                                    .Include(p => p.NameFromNavigation)
                                    .Include(p => p.Status).FirstOrDefault(p => p.Id == SelectedDocuments.Id);
                        if (Name != "")
                            document.Name = Name;
                        if (NameFromNavigation != null)
                            document.NameFrom = context.Organization.Find(NameFromNavigation.IdOrganization).IdOrganization;
                        if (IddocumentNavigation != null && TypeNumber != "")
                        {
                            document.Iddocument = context.Type.FirstOrDefault(p => p.KindId == IddocumentNavigation.Id && p.Number == TypeNumber).DocumentsId;
                            document.Commend = Commend + " от " + CreateDate.ToShortDateString();
                        }
                        if (Path != null)
                            document.Path = Path;
                        if (!Validate())
                        {
                            if (!context.Staffdocuments.Any(p => p.HeadUserId == AuthViewModel.currentUser.UserId))
                                document.User = context.Users.Include(p => p.Staff).FirstOrDefault(p => p.Id == AuthViewModel.currentUser.UserId);
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            context.Documents.Update(document);
                            context.SaveChanges();
                            GetDocumentData();
                            GetUsersData();
                            GetTypeData();
                            GetStatusData();
                            GetOrganizationData();
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данный документ уже присутствует");
                        }
                        ClearFields();
                        GetDocumentData();
                        GetUsersData();
                        GetTypeData();
                        GetStatusData();
                        GetOrganizationData();
                    }
                }, (obj) => SelectedDocuments != null);
            }
        }
        /// <summary>
        /// Команда на использование документа
        /// </summary>
        public ICommand ChangeStatus
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Documents document = new Documents();
                        document = context.Documents.Find(SelectedDocuments.Id);
                        document.UsingDate = DateTime.Now;
                        document.Status = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Используется");
                        context.Documents.Update(document);
                        context.SaveChanges();
                        var temp = new Staffdocuments();
                        temp.Add(AuthViewModel.currentUser.User, SelectedDocuments, Comment);
                        var temp_ = new Register();
                        temp_.Add(AuthViewModel.currentUser.User, document);
                        GetDocumentData();
                        SelectedDocuments = null;
                        DialogHost.CloseDialogCommand.Execute(null, null);
                    }
                });
            }
        }
        /// <summary>
        /// Команда на отправку документа
        /// </summary>
        public ICommand CreateSend
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Documents document = new Documents();
                        var temp = context.Register.Include(p => p.Document).ThenInclude(p => p.Status).Where(p => p.DocumentId == selectedDocuments.Id && p.Document.Status.Status1 == "Проверен");
                        if (temp != null)
                        {
                            context.Register.RemoveRange(temp);
                            context.SaveChanges();
                        }
                        document = context.Documents.Find(SelectedDocuments.Id);
                        document.Status = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Отправлен");
                        document.EditUserId = context.Users.Find(User.Id).Id;
                        context.Documents.Update(document);
                        context.SaveChanges();
                        var temp_ = new Register();
                        temp_.Add(AuthViewModel.currentUser.User, document);
                        GetDocumentData();
                        SelectedDocuments = null;
                        Name = "";
                        DialogHost.CloseDialogCommand.Execute(null, null);
                    }
                }, (obj) => SelectedDocuments != null &&
                User != null &&
                SelectedDocuments.Status.Status1 != "В архиве" &&
                SelectedDocuments.Status.Status1 != "Отправлен" &&
                GetAccess() <= 2);
            }
        }
        /// <summary>
        /// Команда на отправку документа
        /// </summary>
        public ICommand ToSend
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var commend = "Без номера";
                        var Temp = new Models.Type()
                        {
                            KindId = IddocumentNavigation.Id,
                            Number = commend,
                            DateOfExpire = DateTime.Now.AddYears(5)
                        };
                        if (!ValidateType())
                        {
                            context.Type.Add(Temp);
                            context.SaveChanges();
                        }
                        Documents document = new Documents();
                        document = context.Documents.Find(SelectedDocuments.Id);
                        document.AddingDate = DateTime.Now;
                        document.Commend = "от " + DateTime.Now.ToShortDateString();
                        document.Name = "Ответ на документ №" + document.Id + " от " + DateTime.Now.ToShortDateString() + ". " + IddocumentNavigation.KindName + " " + Name;
                        document.Id = 0;
                        document.Status = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Отправлен");
                        document.IddocumentNavigation = context.Type.Include(p => p.Kind).FirstOrDefault(p => p.Number == commend && p.KindId == Temp.KindId);
                        document.NameFrom = NameFromNavigation.IdOrganization;
                        document.EditUserId = context.Users.Find(User.Id).Id;
                        context.Documents.Add(document);
                        context.SaveChanges();
                        var temp_ = new Register();
                        temp_.Add(AuthViewModel.currentUser.User, document);
                        GetDocumentData();
                        SelectedDocuments = null;
                        DialogHost.CloseDialogCommand.Execute(null, null);
                        Name = "";
                    }
                }, (obj) => SelectedDocuments != null &&
                SelectedDocuments.Status.Status1 != "В архиве" &&
                SelectedDocuments.Status.Status1 != "Отправлен" &&
                NameFromNavigation != null &&
                User != null &&
                IddocumentNavigation != null &&
                GetAccess() <= 2);
            }
        }
        /// <summary>
        /// Команда возвращения из архива документа
        /// </summary>
        public ICommand Return
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Documents document = new Documents();
                        document = context.Documents.Include(p=>p.IddocumentNavigation).FirstOrDefault(p=>p.Id == SelectedDocuments.Id);
                        document.Status = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Проверен");
                        var temp = context.Type.Find(document.Iddocument); 
                        temp.DateOfExpire = DateTime.Now.AddDays(14).Date;
                        context.Type.Update(temp);
                        context.SaveChanges();
                        context.Documents.Update(document);
                        context.SaveChanges();
                        var temp_ = new Register();
                        temp_.Add(AuthViewModel.currentUser.User, document);
                        GetDocumentData();
                        SelectedDocuments = null;
                    }
                }, (obj) => SelectedDocuments != null &&
                SelectedDocuments.Status.Status1 == "В архиве" &&
                GetAccess() <= 1);
            }
        }
        /// <summary>
        /// Команда загрузки коллекции внутренних документов
        /// </summary>
        public ICommand GetUsingDocs
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GetUsingDocData();
                }, (obj) =>
                GetAccess() <= 2);
            }
        }
        /// <summary>
        /// Команда загрузки коллекции исходящих документов
        /// </summary>
        public ICommand GetSendingDocs
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GetSendingDocData();
                }, (obj) =>
                GetAccess() <= 2);
            }
        }
        /// <summary>
        /// Команда на архивирование документа
        /// </summary>
        public ICommand ToArchieve
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Documents document = new Documents();
                        document = context.Documents.Find(SelectedDocuments.Id);
                        document.Status = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "В архиве");
                        document.MoveDateToArchieve = DateTime.Now;
                        context.Documents.Update(document);
                        context.SaveChanges();
                        var temp_ = new Register();
                        temp_.Add(AuthViewModel.currentUser.User, document);
                        GetDocumentData();
                        SelectedDocuments = null;
                    }
                }, (obj) => SelectedDocuments != null &&
                SelectedDocuments.Status.Status1 != "В архиве" &&
                SelectedDocuments.Status.Status1 != "Используется" &&
                SelectedDocuments.Status.Status1 != "Отправлен" &&
                GetAccess() <= 1);
            }
        }
        /// <summary>
        /// Удалить выбранный документ
        /// </summary>
        public ICommand Delete
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                if (MessageBox.Show("Вы действительно хотите удалить данный документ?\n(Все данные в регистре будут также удалены!)", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (SelectedDocuments.Status.Status1 == "Отправлен" || SelectedDocuments.Status.Status1 == "В архиве")
                    {
                        using (documentContext context = new documentContext())
                        {
                            Documents document = new Documents();
                            var temp = new Register();
                            document = context.Documents.Find(SelectedDocuments.Id);
                            if (File.Exists(document.Path))
                                try
                                {
                                    File.Delete(document.Path);
                                }
                                catch { }
                                while (context.Register.FirstOrDefault(p => p.DocumentId == document.Id) != null)
                                {
                                    temp = context.Register.FirstOrDefault(p => p.DocumentId == document.Id);
                                    context.Register.Remove(temp);
                                    context.SaveChanges();
                                };
                                context.Documents.Remove(document);
                                context.SaveChanges();
                                GetDocumentData();
                                SelectedDocuments = null;
                            }
                        }
                        else
                        {
                            MessageBox.Show("Можно удалять только отправленные документы!\nВыберите другой документ!", "Предупреждение");
                        }
                    }
                }, (obj) => SelectedDocuments != null &&
                GetAccess() <= 1);
            }
        }
        #endregion
        #region Свойства модели
        #region Публичные свойства
        private ObservableCollection<Documents> _Documents { get; set; }
        public ObservableCollection<Documents> Documents
        {
            get { return _Documents; }
            set
            {
                _Documents = value;
                OnPropertyChanged("Documents");
            }
        }
        private ObservableCollection<Kind> _Types { get; set; }
        public ObservableCollection<Kind> Types
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
        public ObservableCollection<MainMenu> Menus { get; set; }
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
        private DateTime _CreateDate { get; set; }
        public DateTime CreateDate
        {
            get { return _CreateDate; }
            set
            {
                _CreateDate = value;
                OnPropertyChanged("CreateDate");
            }
        }
        private string _SearchTextColor { get; set; }
        public string SearchTextColor
        {
            get { return _SearchTextColor; }
            set
            {
                _SearchTextColor = value;
                OnPropertyChanged("SearchTextColor");
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
        public string Comment
        {
            get { return _Comment; }
            set
            {
                _Comment = value;
                OnPropertyChanged("Comment");
            }
        }
        private string _TypeNumber { get; set; }
        public string TypeNumber { get; set; }
        public DateTime EndingDate
        {
            get { return _EndingDate; }
            set
            {
                _EndingDate = value;
                OnPropertyChanged("EndingDate");
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
        private Kind _IddocumentNavigation { get; set; }
        private Organization _NameFromNavigation { get; set; }
        private Status _Status { get; set; }
        private Users _User { get; set; }
        private Documents selectedDocuments { get; set; }
        private MainMenu _SelectedMenu;
        private string _Comment { get; set; }
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
        public Kind IddocumentNavigation 
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
        public Documents SelectedDocuments
        {
            get { return selectedDocuments; }
            set
            {
                selectedDocuments = value;
                OnPropertyChanged("SelectedDocuments");
            }
        }
        #endregion
        #endregion
        #region Методы модели
        /// <summary>
        /// Получение уровня доступа текущего пользователя
        /// </summary>
        /// <returns>Уровень доступа</returns>
        private int GetAccess()
        {
            using (documentContext context = new documentContext())
            {
                if (AuthViewModel.currentUser != null)
                {
                    var temp = context.Userroles.Include(p => p.Role).Where(p => p.UserId == AuthViewModel.currentUser.UserId).ToList();
                    if (temp.Count == 1)
                        return Convert.ToInt32(AuthViewModel.currentUser.Role.Access);
                    var i = Convert.ToInt32(temp[0].Role.Access);
                    foreach (var item in temp)
                    {
                        if (Convert.ToInt32(item.Role.Access) > i)
                        {
                            i = Convert.ToInt32(item.Role.Access);
                        }
                    }
                    return i;
                }
                else
                {
                    return Int32.MaxValue;
                }
            };
        }
        /// <summary>
        /// Валидация данных
        /// </summary>
        /// <returns>Результат валидации</returns>
        private bool Validate()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Documents.Any(p => (p.Path == Path) && p.Id != SelectedDocuments.Id))
                {
                    return true;
                }
                return false;
            };
        }
        /// <summary>
        /// Валидация данных таблицы типов документов
        /// </summary>
        /// <returns>Результат валидации</returns>
        private bool ValidateType()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Type.Any(p => p.Number == TypeNumber && p.KindId == IddocumentNavigation.Id))
                {
                    return true;
                }
                return false;
            };
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Document, включая данные по внешним ключам
        /// </summary>
        private void GetDocumentData()
        {
            Documents = new ObservableCollection<Documents>();
            int ArchiveCount = 0;
            int usingCount = 0;
            using (documentContext context = new documentContext())
            {
                List<Documents> temp = context.Documents
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.EditUser)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.NameFromNavigation)
                    .Include(p => p.Status)
                    .ToList();
                foreach (var item in temp)
                {
                    if (item.StatusId == context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Отправлен").Id)
                    {
                        ArchiveCount++;
                    }
                    if (item.StatusId == context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "Используется").Id)
                    {
                        usingCount++;
                    }
                    Documents.Add(item);
                }
                Count = "Всего документов: " + temp.Count;
                UsingCount = "Всего используемых документов: " + usingCount.ToString();
                InArchieveCount = "Всего исходящих документов: " + ArchiveCount.ToString();
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные об внутренних документах организации
        /// </summary>
        private void GetUsingDocData()
        {
            Documents = new ObservableCollection<Documents>();
            using (documentContext context = new documentContext())
            {
                List<Documents> temp = context.Documents
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.EditUser)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.NameFromNavigation)
                    .Include(p => p.Status)
                    .Where(p => p.Status.Status1 == "Используется")
                    .ToList();
                foreach (var item in temp)
                {
                    Documents.Add(item);
                }
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные об исходящих документах организации
        /// </summary>
        private void GetSendingDocData()
        {
            Documents = new ObservableCollection<Documents>();
            using (documentContext context = new documentContext())
            {
                List<Documents> temp = context.Documents
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.EditUser)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.NameFromNavigation)
                    .Include(p => p.Status)
                    .Where(p => p.Status.Status1 == "Отправлен")
                    .ToList();
                foreach (var item in temp)
                {
                    Documents.Add(item);
                }
            }
        }
        /// <summary>
        /// Проверка дат для перемещения в архив
        /// </summary>
        private void CheckDates()
        {
            Documents = new ObservableCollection<Documents>();
            using (documentContext context = new documentContext())
            {
                List<Documents> temp = context.Documents
                    .Include(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.NameFromNavigation)
                    .Include(p => p.Status)
                    .ToList();
                foreach (var item in temp)
                {
                    if (item.IddocumentNavigation.DateOfExpire.Date <= DateTime.Now.Date)
                    {
                        item.StatusId = context.Status.AsNoTracking().FirstOrDefault(p => p.Status1 == "В архиве").Id;
                        context.Documents.Update(item);
                        context.SaveChanges();
                    }
                }
            }
        }
        /// <summary>
        /// Загружает в коллекцию данные из таблицы Type, включая данные по внешним ключам
        /// </summary>
        private void GetTypeData()
        {
            Types = new ObservableCollection<Kind>();
            using (documentContext context = new documentContext())
            {
                List<Kind> temp = context.Kind.ToList();
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
            var temp = Documents.Where(p => p.Status.Status1 == text || 
                    p.NameFromNavigation.Name == text || 
                    p.Name == text || 
                    p.IddocumentNavigation.Number == text || 
                    p.IddocumentNavigation.Kind.KindName == text)
                .ToList();
            Documents = new ObservableCollection<Documents>();
            foreach (var item in temp)
            {
                Documents.Add(item);
            }
        }
        /// <summary>
        /// Очистка полей
        /// </summary>
        private void ClearFields()
        {
            Name = "";
            Path = "";
            NameFromNavigation = null;
            TypeNumber = "";
            IddocumentNavigation = null;
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
