using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace AccoutingDocs.ViewModels
{
    public class RolesViewModel : INotifyPropertyChanged
    {
        public RolesViewModel()
        {
            GetData();
        }
        private ObservableCollection<Userroles> _Userroles;
        public ObservableCollection<Userroles> Userroles
        {
            get { return _Userroles; }
            set
            {
                _Userroles = value;
                OnPropertyChanged("Userroles");
            } 
        }
        private ObservableCollection<Users> _Users;
        public ObservableCollection<Users> Users
        {
            get { return _Users; }
            set
            {
                _Users = value;
                OnPropertyChanged("Users");
            }
        }
        private ObservableCollection<Roles> _Roles;
        public ObservableCollection<Roles> Roles
        {
            get { return _Roles; }
            set
            {
                _Roles = value;
                OnPropertyChanged("Roles");
            }
        }
        public Roles DataRole 
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                OnPropertyChanged("SelectedRole");
            }
        }
        /// <summary>
        /// Команда на добавление новой роли
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Add();
                    Clear();
                }, (obj) => SelectedRole == null && Name != null && SelectedAccess != null);
            }
        }
        /// <summary>
        /// Команда редактирования роли
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var id = context.Roles.Where(p => p.Name == Name).AsNoTracking().FirstOrDefault();
                        if (id != null)
                        {
                            if (id.Id == SelectedRole.Id)
                            {
                                Edit();
                                SelectedRole = DataRole;
                            }
                            else
                            {
                                MessageBox.Show("Данная роль уже есть");
                            }
                        }
                        else
                        {
                            Edit();
                            SelectedRole = DataRole;
                        }
                        Clear();
                    }
                }, (obj) => SelectedRole != null && 
                Name != null &&
                SelectedRole.Name != "Администратор" &&
                SelectedRole.Name != "Руководитель" &&
                SelectedRole.Name != "Секретарь" &&
                SelectedRole.Name != "IT-сотрудник");
            }
        }
        /// <summary>
        /// Команда удаления роли
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Roles roles = obj as Roles;
                    if (roles != null)
                    {
                        using (documentContext context = new documentContext())
                        {
                            if (MessageBox.Show("Вы действительно хотите удалить данную роль?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
                                var removingRoles = context.Roles.Where(p => p.Name == SelectedRole.Name).FirstOrDefault();
                                if (!context.Userroles.Where(p => p.RoleId == SelectedRole.Id).Any())
                                {
                                    context.Roles.Remove(removingRoles);
                                    context.SaveChanges();
                                    SelectedRole = null;
                                    GetData();
                                }
                                else
                                {
                                    MessageBox.Show("Данная роль используется! Удаление невозможно!", "Ошибка!");
                                    SelectedRole = null;
                                }
                            }
                        }
                    }
                }, (obj) => SelectedRole != null && 
                SelectedRole.Name != "Администратор" && 
                SelectedRole.Name != "Руководитель" && 
                SelectedRole.Name != "Секретарь" &&
                SelectedRole.Name != "IT-сотрудник");
            }
        }
        /// <summary>
        /// Команда на добавление роли пользователю
        /// </summary>
        public ICommand AddRoleCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {

                        if (!context.Userroles.Where(p => p.UserId == CurrentUser.Id && p.RoleId == AddingRole.Id).Any())
                        {
                            Userroles role = new Userroles();
                            role.User = context.Users.Find(CurrentUser.Id);
                            role.Role = context.Roles.Find(AddingRole.Id);
                            context.Userroles.Add(role);
                            context.SaveChanges();
                            GetData();
                        }
                        else
                        {
                            MessageBox.Show("Данная роль у выбранного пользователя уже существует!", "Ошибка!");
                        }
                        AddingRole = null;
                        CurrentUser = null;
                    }
                }, (obj) => AddingRole != null && CurrentUser != null);
            }
        }
        /// <summary>
        /// Команда на удаление роли пользователю
        /// </summary>
        public ICommand DeleteRoleCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        if (MessageBox.Show("Вы действительно хотите удалить данную роль у выбранного пользователя?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            var temp = context.Userroles.FirstOrDefault(p => p.UserId == CurrentRoles.UserId);
                            if (temp.UserId != AuthViewModel.currentUser.UserId)
                            {
                                if (CurrentRoles.Role.Name != "Администратор" && AuthViewModel.currentUser.Role.Name == "Администратор")
                                {
                                    context.Userroles.Remove(temp);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    MessageBox.Show("У вас нет прав, чтобы снять данную роль!", "Ошибка");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Нельзя удалить роль у самого себя!", "Ошибка");
                            }
                        }
                        CurrentRoles = null;
                        GetData();
                    }
                }, (obj) => CurrentRoles != null);
            }
        }
        private string _Name { get; set; }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        private Roles selectedRole;
        public Roles SelectedRole
        {
            get { return selectedRole; }
            set
            {
                selectedRole = value;
                if (selectedRole != null)
                    GetRole();
                OnPropertyChanged("SelectedRole");
            }
        }
        private Userroles currentRoles;
        public Userroles CurrentRoles
        {
            get { return currentRoles; }
            set
            {
                currentRoles = value;
                OnPropertyChanged("CurrentRoles");
            }
        }
        private Users currentUser;
        public Users CurrentUser
        {
            get { return currentUser; }
            set
            {
                currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }
        private Roles addingRole;
        public Roles AddingRole
        {
            get { return addingRole; }
            set
            {
                addingRole = value;
                OnPropertyChanged("AddingRole");
            }
        }
        private Access _SelectedAccess;
        public Access SelectedAccess
        {
            get
            {
                return _SelectedAccess;
            }
            set
            {
                _SelectedAccess = value;
                OnPropertyChanged("SelectedAccess");
            }
        }
        private ObservableCollection<Access> _Accesses;
        public ObservableCollection<Access> Accesses
        {
            get
            {
                return _Accesses;
            }
            set
            {
                _Accesses = value;
                OnPropertyChanged("Accesses");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName]string property = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            }
        }
        /// <summary>
        /// Получение данных из таблиц
        /// </summary>
        private void GetData()
        {
            using (documentContext context = new documentContext())
            {
                Userroles = new ObservableCollection<Userroles>();
                Users = new ObservableCollection<Users>();
                Roles = new ObservableCollection<Roles>();
                List<Roles> temp = context.Roles.ToList();
                foreach (var item in temp)
                {
                    Roles.Add(item);
                }
                List<Users> users = context.Users.ToList();
                foreach (var item in users)
                {
                    Users.Add(item);
                }
                List<Userroles> userroles = context.Userroles.Include(p => p.User).Include(p => p.Role).ToList();
                foreach (var item in userroles)
                {
                    Userroles.Add(item);
                }
                Accesses = Access.GetAccesses();
            }
        }
        /// <summary>
        /// Получение ролей
        /// </summary>
        private void GetRole()
        {
            Name = SelectedRole.Name;
            SelectedAccess = Accesses.FirstOrDefault(p => p.AccessID.ToString() == SelectedRole.Access);
        }
       /// <summary>
       /// Очистка полей
       /// </summary>
        private void Clear()
        {
            Name = "";
            SelectedAccess = null;
            SelectedRole = null;
        }
        /// <summary>
        /// Редактирование ролей
        /// </summary>
        private void Edit()
        {
            using (documentContext context = new documentContext())
            {
                Roles roles = new Roles();
                roles.Id = SelectedRole.Id;
                roles.Name = Name;
                if(SelectedAccess != null)
                    roles.Access = SelectedAccess.AccessID.ToString();
                Roles[Roles.IndexOf(SelectedRole)] = roles;
                SelectedRole = roles;
                context.Roles.Update(SelectedRole);
                context.SaveChanges();
                Clear();
            }
        }
        /// <summary>
        /// Добавление ролей
        /// </summary>
        private void Add()
        {
            using (documentContext context = new documentContext())
            {
                if (!context.Roles.Where(p => p.Name == Name).Any())
                {
                    Roles role = new Roles
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = Name,
                        Access = SelectedAccess.AccessID.ToString()
                    };
                    context.Roles.Add(role);
                    context.SaveChanges();
                    Roles.Add(role);
                }
                else
                {
                    MessageBox.Show("Данная роль уже присутствует!\n Отмена операции.", "Ошибка!");
                }
            }
        }
    }
}
