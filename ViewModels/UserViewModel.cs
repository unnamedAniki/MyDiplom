using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    public class UserViewModel : INotifyPropertyChanged
    {
        public UserViewModel()
        {
            GetData();
            Clear();
        }
        #region Команды добавления/редактирования/удаления
        /// <summary>
        /// Команда открытия диалогового окна на добавление в таблицу Users 
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DialogHost.OpenDialogCommand.Execute(null, null);
                    GetData();
                    SelectedUser = null;
                });
            }
        }
        /// <summary>
        /// Команда добавления
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Users users = new Users()
                        {
                            Id = Guid.NewGuid().ToString(),
                            UserName = UserName,
                            PasswordHash = Supports.PasswordHasher.HashPassword(PasswordHash),
                            Email = Email,
                            Name = Name,
                            Fam = Fam,
                            Lastname = LastName,
                            Staff = context.Staff.Find(SelectedStaff.Id)
                        };
                        context.Users.Add(users);
                        context.SaveChanges();
                        GetData();
                        Clear();
                    }
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }, (obj) => SelectedUser == null && UserName != null && Email != null && SelectedStaff != null);
            }
        }
        
        /// <summary>
        /// Команда редактирования таблицы Users 
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var user = context.Users.Find(SelectedUser.Id);
                        if (UserName != null)
                        {
                            user.UserName = UserName;
                        }
                        if (PasswordHash != "" && PasswordConfirm != "")
                        {
                            user.PasswordHash = Supports.PasswordHasher.HashPassword(PasswordHash);
                        }
                        if (Email != null)
                        {
                            user.Email = Email;
                        }
                        if (Name != null)
                        {
                            user.Name = Name;
                        }
                        if (LastName != null)
                        {
                            user.Lastname = LastName;
                        }
                        if (Fam != null)
                        {
                            user.Fam = Fam;
                        }
                        if (SelectedStaff != null)
                        {
                            user.Staff = context.Staff.Find(SelectedStaff.Id);
                            user.StaffId = SelectedStaff.Id;
                        }
                        if (!context.Users.Where(p => (p.UserName == UserName || p.Email == Email) && p.Id != user.Id).Any()) 
                        {
                            context.Users.Update(user);
                            context.SaveChanges();
                        }
                        GetData();
                    }
                    DialogHost.CloseDialogCommand.Execute(null, null);
                    Clear();
                }, (obj) => SelectedUser != null);
            }
        }
        /// <summary>
        /// Команда открытия диалогового окна на редактирование в таблицу Users 
        /// </summary>
        public ICommand OpenEditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    UserName = SelectedUser.UserName;
                    Email = SelectedUser.Email;
                    Name = SelectedUser.Name;
                    Fam = SelectedUser.Fam;
                    LastName = SelectedUser.Lastname;
                    SelectedStaff = SelectedUser.Staff;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                },(obj) => SelectedUser != null);
            }
        }
        /// <summary>
        /// Команда удаления из таблицы Users 
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        if (MessageBox.Show("Вы действительно хотите удалить данного пользователя?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            if (!Validate())
                            {
                                var temp = context.Users.Where(p => p.Id == SelectedUser.Id).FirstOrDefault();
                                context.Users.Remove(temp);
                                context.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("Данный пользователь используется в других записях! Удаление невозможно!", "Ошибка!");
                            }
                        }
                        GetData();
                        SelectedUser = null;
                    }
                }, (obj) => SelectedUser != null);
            }
        }

        #endregion
        #region Свойства UserViewModel
        private ObservableCollection<Users> _User { get; set; }
        public ObservableCollection<Users> User
        {
            get { return _User; }
            set
            {
                _User = value;
                OnPropertyChanged("User");
            }
        }
        public ObservableCollection<Staff> _Staffs { get; set; }
        public ObservableCollection<Staff> Staffs
        {
            get { return _Staffs; }
            set
            {
                _Staffs = value;
                OnPropertyChanged("Staffs");
            }
        }
        private string _Id { get; set; }
        private string _UserName { get; set; }
        private string _Email { get; set; }
        private string _PasswordHash { get; set; }
        private string _PasswordConfirm { get; set; }
        private string _Name { get; set; }
        private string _Fam { get; set; }
        private string _LastName { get; set; }
        private string _Staff_Name { get; set; }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Fam
        {
            get { return _Fam; }
            set
            {
                _Fam = value;
                OnPropertyChanged("Fam");
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
        public string Id
        {
            get { return _Id; }
            set
            {
                _Id = value;
                OnPropertyChanged("Id");
            }
        }
        public string UserName
        {
            get { return _UserName; }
            set
            {
                _UserName = value;
                OnPropertyChanged("UserName");
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
        public string PasswordHash
        {
            get { return _PasswordHash; }
            set
            {
                _PasswordHash = value;
                OnPropertyChanged("PasswordHash");
            }
        }
        public string PasswordConfirm
        {
            get { return _PasswordConfirm; }
            set
            {
                _PasswordConfirm = value;
                OnPropertyChanged("PasswordConfirm");
            }
        }
        public string Staff_Name
        {
            get { return _Staff_Name; }
            set
            {
                _Staff_Name = value;
                OnPropertyChanged("Staff_Name");
            }
        }
        private Staff selectedStaff;
        private Users selectedUser;
        public Staff SelectedStaff
        {
            get { return selectedStaff; }
            set
            {
                selectedStaff = value;
                OnPropertyChanged("SelectedStaff");
            }
        }
        public Users SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }
        #endregion
        /// <summary>
        /// Получение данных из таблицы Пользователи в коллекцию 
        /// </summary>
        private void GetData()
        {
            User = new ObservableCollection<Users>();
            Staffs = new ObservableCollection<Staff>();
            using (documentContext context = new documentContext())
            {
                List<Users> temp = context.Users.Include(p => p.Staff).ToList();
                foreach (var item in temp)
                {
                    User.Add(item);
                }
                List<Staff> collection = context.Staff.ToList();
                foreach (var item in collection)
                {
                    Staffs.Add(item);
                }
            }
        }
        /// <summary>
        /// Валидация данных
        /// </summary>
        private bool Validate()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Documents.Where(p => p.UserId == SelectedUser.Id || p.EditUserId == SelectedUser.Id).Any() ||
                    context.Register.Where(p => p.UserId == SelectedUser.Id).Any() ||
                    context.Staffdocuments.Where(p => p.HeadUserId == SelectedUser.Id).Any() ||
                    context.Userroles.Where(p => p.UserId == SelectedUser.Id).Any())
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Очистка полей
        /// </summary>
        private void Clear()
        {
            UserName = "";
            PasswordHash = "";
            PasswordConfirm = "";
            Email = "";
            Name = "";
            LastName = "";
            Fam = "";
            SelectedStaff = null;
            SelectedUser = null;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
