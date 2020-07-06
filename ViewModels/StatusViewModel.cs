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
    public class StatusViewModel : INotifyPropertyChanged
    {
        public StatusViewModel()
        {
            GetStatus();
        }
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        if (!context.Status.Where(p=>p.Status1 == Status1).Any())
                        {

                            Status status = new Status();
                            status.Status1 = Status1;
                            Statuses.Add(status);
                            SelectedStatus = status;
                            context.Status.Add(SelectedStatus);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данный статус уже есть");
                        }
                    }
                    Status1 = "";
                    SelectedStatus = null;
                }, (obj) => SelectedStatus == null && Status1 != null);
            }
        }
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (SelectedStatus != null)
                    {
                        using (documentContext context = new documentContext())
                        {
                            var id = context.Status.AsNoTracking().Where(p => p.Status1 == Status1).FirstOrDefault();
                            if (id != null)
                            {
                                if (context.Status.AsNoTracking().Where(p => p.Status1 == Status1).FirstOrDefault().Id == SelectedStatus.Id)
                                {
                                    Status Status = new Status();
                                    Status.Id = SelectedStatus.Id;
                                    Status.Status1 = Status1;
                                    Statuses[Statuses.IndexOf(SelectedStatus)] = Status;
                                    SelectedStatus = Status;
                                    context.Status.Update(SelectedStatus);
                                    context.SaveChanges();
                                }
                                else
                                {
                                    MessageBox.Show("Данный статус уже есть");
                                }
                            }
                            else
                            {
                                Status Status = new Status();
                                Status.Id = SelectedStatus.Id;
                                Status.Status1 = Status1;
                                Statuses[Statuses.IndexOf(SelectedStatus)] = Status;
                                SelectedStatus = Status;
                                context.Status.Update(SelectedStatus);
                                context.SaveChanges();
                            }
                        }
                        Status1 = "";
                        SelectedStatus = null;
                    }
                }, (obj) => SelectedStatus != null &&
                            SelectedStatus.Status1 != "Отправлен" &&
                            SelectedStatus.Status1 != "Используется" &&
                            SelectedStatus.Status1 != "Проверен" &&
                            SelectedStatus.Status1 != "В архиве");
            }
        }
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (MessageBox.Show("Вы действительно хотите удалить данный статус?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (!ValidateStatus())
                        {
                            using (documentContext context = new documentContext())
                            {
                                context.Status.Remove(SelectedStatus);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный статус уже используется! Удаление невозможно!", "Ошибка!");
                        }
                    }
                    GetStatus();
                    Status1 = "";
                    SelectedStatus = null;
                }, (obj) => SelectedStatus != null && 
                            SelectedStatus.Status1 != "Отправлен" &&
                            SelectedStatus.Status1 != "Используется" &&
                            SelectedStatus.Status1 != "Проверен" &&
                            SelectedStatus.Status1 != "В архиве"
                );
            }
        }
        private string _Status1 { get; set; }
        public string Status1
        {
            get { return _Status1; }
            set
            {
                _Status1 = value;
                OnPropertyChanged("Status1");
            }
        }
        private Status selectedStatus;
        public Status SelectedStatus
        {
            get { return selectedStatus; }
            set
            {
                selectedStatus = value; ;
                OnPropertyChanged("SelectedStatus");
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
        private void GetStatus()
        {
            Statuses = new ObservableCollection<Status>();
            using (documentContext context = new documentContext())
            {
                List<Status> temp = context.Status.ToList();
                foreach (var item in temp)
                {
                    Statuses.Add(item);
                }
            }
        }
        private bool ValidateStatus()
        {
            using (documentContext context = new documentContext())
            {

                if (SelectedStatus != null)
                    return (context.Documents.Where(p => p.StatusId == SelectedStatus.Id).Any());
                return false; 
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
