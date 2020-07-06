using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    public class ManageViewModel : INotifyPropertyChanged
    {
        public ManageViewModel()
        {
            CurrentUser = AuthViewModel.currentUser.User;
            GetData();
            Fam = CurrentUser.Fam;
            Name = CurrentUser.Name;
            LastName = CurrentUser.Lastname;
            Email = CurrentUser.Email;
        }
        #region Команды
        /// <summary>
        /// Команда отмены изменений
        /// </summary>
        public ICommand Cancel
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Fam = CurrentUser.Fam;
                    Name = CurrentUser.Name;
                    LastName = CurrentUser.Lastname;
                    Email = CurrentUser.Email;
                });
            }
        }
        /// <summary>
        /// Команда примeнения изменений
        /// </summary>
        public ICommand Apply
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Users user = new Users();
                        user = CurrentUser;
                        user.Fam = Fam;
                        user.Name = Name;
                        user.Lastname = LastName;
                        user.Email = Email;
                        context.Users.Update(user);
                        context.SaveChanges();
                    }
                });
            }
        }
        /// <summary>
        /// Команда открытия диалогового окна
        /// </summary>
        public ICommand OpenConfirm
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DialogHost.OpenDialogCommand.Execute(null, null);
                }, (obj) => SelectedDoc != null);
            }
        }
        /// <summary>
        /// Команда подтверждения выполнения
        /// </summary>
        public ICommand Confirm
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var temp = new Documents();
                        context.Staffdocuments.Remove(SelectedDoc);
                        context.SaveChanges();
                        temp = context.Documents.Find(SelectedDoc.DocumentId);
                        temp.StatusId = context.Status.FirstOrDefault(p => p.Status1 == "Проверен").Id;
                        temp.EditUserId = AuthViewModel.currentUser.UserId;
                        context.Documents.Update(temp);
                        context.SaveChanges();
                    }
                    SelectedDoc = null;
                    GetData();
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }, (obj) => SelectedDoc != null);
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
                    GetData();
                });
            }
        }
        #endregion
        #region Приватные свойства
        private string _Fam { get; set; }
        private string _Name { get; set; }
        private string _LastName { get; set; }
        private string _Email { get; set; }
        private Users _CurrentUser { get; set; }
        private ObservableCollection<Documents> _Documents { get; set; }
        private Users _HeadUser { get; set; }
        private Staffdocuments _SelectedDoc { get; set; }
        private ObservableCollection<Staffdocuments> _AllDocuments { get; set; }
        #endregion
        #region Публичные свойства
        public string Fam
        {
            get { return _Fam; }
            set
            {
                _Fam = value;
                OnPropertyChanged("Fam");
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
        public string LastName
        {
            get { return _LastName; }
            set
            {
                _LastName = value;
                OnPropertyChanged("LastName");
            }
        }
        public string Email
        {
            get { return _Email; }
            set
            {
                _Email = value;
                OnPropertyChanged("Email");
            }
        }
        public Users CurrentUser 
        {
            get { return _CurrentUser; }
            set
            {
                _CurrentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }
        public ObservableCollection<Documents> Documents
        {
            get { return _Documents; }
            set
            {
                _Documents = value;
                OnPropertyChanged("Documents");
            }
        }
        public Users HeadUser
        {
            get { return _HeadUser; }
            set
            {
                _HeadUser = value;
                OnPropertyChanged("HeadUser");
            }
        }
        public Staffdocuments SelectedDoc
        {
            get { return _SelectedDoc; }
            set
            {
                _SelectedDoc = value;
                OnPropertyChanged("_SelectedDoc");
            }
        }
        public ObservableCollection<Staffdocuments> AllDocuments
        {
            get { return _AllDocuments; }
            set
            {
                _AllDocuments = value;
                OnPropertyChanged("AllDocuments");
            }
        }
        #endregion
        #region Процедуры и функции ViewModel
        /// <summary>
        /// Получение списка используемых документов
        /// </summary>
        private void GetData()
        {
            AllDocuments = new ObservableCollection<Staffdocuments>();
            using (documentContext context = new documentContext())
            {
                var temp = context.Staffdocuments
                    .Include(p => p.Document)
                    .ThenInclude(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.Status)
                    .Include(p => p.HeadUser)
                    .Where(p => p.HeadUser.Staff == CurrentUser.Staff)
                    .ToList();
                foreach (var item in temp)
                {
                    AllDocuments.Add(item);
                }
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
