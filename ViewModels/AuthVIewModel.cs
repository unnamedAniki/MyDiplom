using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        public AuthViewModel()
        {

        }
        #region Свойства с работой авторизацией
        /// <summary>
        /// Авторизация
        /// </summary>
        public ICommand Auth
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var values = (object[])obj;
                    var password = values[0] as PasswordBox;
                    App.main = (MainWindow)values[1];
                    try
                    {
                        using (documentContext context = new documentContext())
                        {
                            if (context.Users.Where(u => u.UserName == Login).FirstOrDefault() != null)
                            {
                                var pass = context.Users.Include(p => p.Userroles).Where(u => u.UserName == Login).FirstOrDefault().PasswordHash;
                                if (!String.IsNullOrEmpty(pass))
                                {
                                    Password = password.Password;
                                    var result = Supports.PasswordHasher.VerifyHashedPassword(pass, Password);
                                    if (result)
                                    {
                                        currentUser = null;
                                        var temp = context.Userroles.Include(p => p.User).Include(p => p.Role).Where(p => p.User.UserName == Login).FirstOrDefault();
                                        if (temp != null)
                                        {
                                            if (temp.Role == null)
                                            {
                                                MessageBox.Show("Данная учетная запись еще не активирована!\nОбратитесь к администратору за получением доступа");
                                            }
                                            else
                                            {
                                                currentUser = context.Userroles.Include(p => p.User).ThenInclude(p => p.Staff).Include(p => p.Role).Where(p => p.User.UserName == Login).FirstOrDefault();
                                                GetRole();
                                                Role = currentUser.Role.Name;
                                                App.main.Hide();
                                                HeadMenu head = new HeadMenu();
                                                head.Show();
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Данная учетная запись еще не активирована!\nОбратитесь к администратору за получением доступа");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Неправильное имя пользователя или пароль!");
                                    }
                                }
                            }
                            else
                            {
                                MessageBox.Show("Неправильное имя пользователя или пароль!");
                            }
                        }
                    }
                    catch { };
                });
            }
        }
        public static string Role = "";
        public static Userroles currentUser;
        private Window _Window { get; set; }
        public Window Window
        {
            get { return _Window; }
            set
            {
                _Window = value;
                OnPropertyChanged("Window");
            }
        }
        private string _Login { get; set; }
        public string Login
        {
            get { return _Login; }
            set
            {
                _Login = value;
                OnPropertyChanged("Login");
            }
        }
        private string _Password { get; set; }
        public string Password
        {
            get { return _Password; }
            set
            {
                _Password = value;
                OnPropertyChanged("Password");
            }
        }
        private string _Title { get; set; }
        public string Title
        {
            get { return _Title; }
            set
            {
                _Title = value;
                OnPropertyChanged("Title");
            }
        }
        /// <summary>
        /// Получение роли
        /// </summary>
        private void GetRole()
        {
            using (documentContext context = new documentContext())
            {
                var temp = context.Userroles.Include(p => p.Role).Where(p => p.UserId == currentUser.UserId).ToList();
                if(temp.Count != 1)
                {
                    currentUser.Role = temp.Last().Role;
                }
            }
        }
        #endregion
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
