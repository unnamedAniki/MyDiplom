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
using System.Windows;
using Microsoft.EntityFrameworkCore;

namespace AccoutingDocs.ViewModels
{
    public class StaffsViewModel : INotifyPropertyChanged
    {
        public StaffsViewModel()
        {
            GetData();
        }
        private Staff selectedStaffs;
        private ObservableCollection<Staff> _staffnames { get; set; }
        public ObservableCollection<Staff> staffnames
        {
            get { return _staffnames; }
            set 
            {
                _staffnames = value;
                OnPropertyChanged("staffnames");
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
                        SelectedStaffs = null;
                        if (!context.Staff.Where(p => p.Staff_Name == Staff_Name).Any())
                        {
                            var staff = new Staff();
                            staff.Staff_Name = Staff_Name;
                            context.Staff.Add(staff);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данное подразделение уже есть", "Ошибка!");
                        }
                    }
                    Staff_Name = "";
                    SelectedStaffs = null;
                    GetData();
                }, (obj) => Staff_Name != null);
            }
        }
        /// <summary>
        /// Команда редактирования
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var staff = context.Staff.Find(SelectedStaffs.Id);        
                        staff.Staff_Name = Staff_Name;
                        if (!context.Staff.Where(p => p.Staff_Name == Staff_Name && p.Id != staff.Id).Any())
                        {
                            context.Staff.Update(staff);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данное подразделение уже есть", "Ошибка!");
                        }
                    }
                    Staff_Name = "";
                    SelectedStaffs = null;
                    GetData();
                }, (obj) => SelectedStaffs != null && Staff_Name != "");
            }
        }
        /// <summary>
        /// Команда удаления
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (MessageBox.Show("Вы действительно хотите удалить данное подразделение?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (!Validate())
                        {
                            using (documentContext context = new documentContext())
                            {
                                context.Staff.Remove(SelectedStaffs);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данное подразделение уже используется! Удаление невозможно!","Ошибка!");
                        }
                    }
                    GetData();
                    Staff_Name = "";
                    SelectedStaffs = null;
                }, (obj) => SelectedStaffs != null);
            }
        }
        private string _Staff_Name { get; set; }
        public string Staff_Name 
        {
            get { return _Staff_Name; }
            set
            {
                _Staff_Name = value;
                OnPropertyChanged("Staff_Name");
            }
        }
        public Staff SelectedStaffs
        {
            get { return selectedStaffs; }
            set
            {
                selectedStaffs = value;
                if(value != null)
                    Staff_Name = value.Staff_Name;
                OnPropertyChanged("SelectedStaffs");
            }
        }
        /// <summary>
        /// Валиидация данных
        /// </summary>
        private bool Validate()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Users.Where(p => p.StaffId == SelectedStaffs.Id).Any())
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Получаение списка подразделений из таблицы
        /// </summary>
        private void GetData()
        {
            staffnames = new ObservableCollection<Staff>();
            using (documentContext context = new documentContext())
            {
                List<Staff> temp = context.Staff.ToList();
                foreach (var item in temp)
                {
                    staffnames.Add(item);
                }
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
    }
}
