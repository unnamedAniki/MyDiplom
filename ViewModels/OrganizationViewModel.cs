using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using AccoutingDocs.Commands;
using AccoutingDocs.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    /// <summary>
    /// Логика взаимодействия с формой Организация
    /// </summary>
    public class OrganizationViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Конструктор класса
        /// </summary>
        public OrganizationViewModel()
        {
            GetData();
            HeadName = "";
            Name = "";
            Mail = "";
        }
        #region Команды модели
        /// <summary>
        /// Команда открытия диалогового окна
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DialogHost.OpenDialogCommand.Execute(null, null);
                    SelectedOrganization = null;
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
                        Organization temp = new Organization()
                        {
                            Name = Name,
                            Mail = Mail,
                            HeadName = HeadName
                        };
                        if (!context.Organization.Where(p => Name == Name || p.Mail == Mail).Any())
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            context.Organization.Add(temp);
                            context.SaveChanges();
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данная организация уже присутствует!", "Ошибка!");
                        }
                        GetData();
                    }
                }, (obj) => SelectedOrganization == null);
            }
        }

        /// <summary>
        /// Команда редактирования таблицы Organization 
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Organization temp = new Organization();
                        temp = context.Organization.Find(SelectedOrganization.IdOrganization);
                        temp.Name = Name;
                        temp.Mail = Mail;
                        temp.HeadName = HeadName;
                        if (!context.Organization.Where(p => (p.Name == Name || p.Mail == Mail) && p.IdOrganization != temp.IdOrganization).Any())
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            context.Organization.Update(temp);
                            context.SaveChanges();
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данная организация уже присутствует!", "Ошибка!"); 
                        }
                    }
                    Name = "";
                    Mail = "";
                    HeadName = "";
                    SelectedOrganization = null;
                    GetData();
                }, (obj) => SelectedOrganization != null && Name != null && Mail != null && HeadName != "");
            }
        }
        /// <summary>
        /// Команда открытия диалогового окна на редактирование
        /// </summary>
        public ICommand OpenEditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Name = SelectedOrganization.Name;
                    Mail = SelectedOrganization.Mail;
                    HeadName = SelectedOrganization.HeadName;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                }, (obj) => SelectedOrganization != null);
            }
        }
        /// <summary>
        /// Команда удаления из таблицы Organization 
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        if (MessageBox.Show("Вы действительно хотите удалить данную организацию?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            if (!Validate())
                            {
                                var temp = context.Organization.Find(SelectedOrganization.IdOrganization); ;
                                context.Organization.Remove(temp);
                                context.SaveChanges();
                                GetData();
                            }
                            else
                            {
                                MessageBox.Show("Данная организация уже используется! Удаление невозможно!");
                            }
                        }
                    }
                    SelectedOrganization = null;
                }, (obj) => SelectedOrganization != null);
            }
        }
        #endregion
        #region Свойства Модели
        private ObservableCollection<Organization> _Organizations { get; set; }
        private Organization selectedOrganization { get; set; }
        private string _Mail { get; set; }
        private string _Name { get; set; }
        private string _HeadName { get; set; }
        public ObservableCollection<Organization> Organizations
        {
            get
            {
                return _Organizations;
            }
            set
            {
                _Organizations = value;
                OnPropertyChanged("Organizations");
            }
        }
        public Organization SelectedOrganization
        {
            get { return selectedOrganization; }
            set
            {
                selectedOrganization = value;
                OnPropertyChanged("SelectedOrganization");
            }
        }
        public string Mail
        {
            get
            {
                return _Mail;
            }
            set
            {
                _Mail = value;
                OnPropertyChanged("Mail");
            }
        }
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
                OnPropertyChanged("Name");
            }
        }
        public string HeadName
        {
            get
            {
                return _HeadName;
            }
            set
            {
                _HeadName = value;
                OnPropertyChanged("HeadName");
            }
        }
        #endregion
        #region Приватные методы модели
        /// <summary>
        /// Получения данных из таблицы Организации в коллекцию
        /// </summary>
        private void GetData()
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
        /// Валидация
        /// </summary>
        private bool Validate()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Documents
                    .Include(p => p.NameFromNavigation)
                    .Where(p => p.NameFromNavigation.Name == SelectedOrganization.Name)
                    .Any())
                    return true;
                return false;
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
