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
    public class TypeViewModel : INotifyPropertyChanged
    {
        public TypeViewModel()
        {
            GetData();
            GetKinds();
            DateOfExpire = DateTime.Today.Date;
        }
        #region Команды модели
        /// <summary>
        /// Команда на открытия диалогового окна
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DateOfExpire = DateTime.Today.Date;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                    SelectedType = null;
                });
            }
        }
        /// <summary>
        /// Команда обновления данных
        /// </summary>
        public ICommand Refresh
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    GetData();
                    GetKinds();
                    if(SelectedType != null)
                    {
                        Kind = SelectedType.Kind;
                    }
                });
            }
        }
        /// <summary>
        /// Команда добавления в таблицу Type
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        Models.Type temp = new Models.Type()
                        {
                            Kind = context.Kind.Find(Kind.Id),
                            Number = Number,
                            DateOfExpire = DateOfExpire
                        };
                        if (!context.Type.Where(p => p.KindId == Kind.Id && p.Number == Number).Any())
                        {
                            context.Type.Add(temp);
                            context.SaveChanges();
                            DialogHost.CloseDialogCommand.Execute(null, null);
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данный номер уже присутствует!", "Ошибка!");
                        }
                        Clear();
                        GetData();
                        GetKinds();
                    }
                }, (obj) => SelectedType == null && Kind != null && Number != null);
            }
        }
        /// <summary>
        /// Команда редактирования таблицы Type 
        /// </summary>
        public ICommand EditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    using (documentContext context = new documentContext())
                    {
                        var temp = context.Type.Find(SelectedType.DocumentsId);
                        if (Kind != null)
                        {
                            temp.Kind = context.Kind.Find(Kind.Id);
                        }
                        if (Number != null)
                        {
                            temp.Number = Number;
                        }
                        if (DateOfExpire != null)
                        {
                            temp.DateOfExpire = DateOfExpire;
                        }
                        if (!context.Type.Where(p => p.KindId == Kind.Id && p.Number == Number && p.DocumentsId != temp.DocumentsId).Any())
                        {
                            context.Type.Update(temp);
                            context.SaveChanges();
                            DialogHost.CloseDialogCommand.Execute(null, null);
                        }
                        else
                        {
                            DialogHost.CloseDialogCommand.Execute(null, null);
                            MessageBox.Show("Данный номер уже присутствует!", "Ошибка!");
                        }
                        GetData();
                        GetKinds();
                        Clear();
                    }
                }, (obj) => SelectedType != null);
            }
        }
        public ICommand OpenEditCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    Number = SelectedType.Number;
                    DateOfExpire = SelectedType.DateOfExpire;
                    Kind = SelectedType.Kind;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                }, (obj) => SelectedType != null);
            }
        }
        /// <summary>
        /// Команда удаления из таблицы Type 
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (MessageBox.Show("Вы действительно хотите удалить данный тип документа?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (!Validate())
                        {
                            using (documentContext context = new documentContext())
                            {
                                var temp = context.Type.Include(p => p.Kind).Where(p => p.Kind == SelectedType.Kind && p.Number == SelectedType.Number).FirstOrDefault();
                                context.Type.Remove(temp);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный тип документа используется! Удаление невозможно!", "Ошибка!");
                        }
                    }
                    GetData();
                    GetKinds();
                    Clear();
                }, (obj) => SelectedType != null);
            }
        }
        #endregion
        #region Свойства Модели
        private ObservableCollection<Kind> _Kinds { get; set; }
        private ObservableCollection<Models.Type> _Types { get; set; }
        private Models.Type selectedType { get; set; }
        private Kind _Kind { get; set; }
        private string _Number { get; set; }
        private DateTime _DateOfExpire { get; set; }
        public ObservableCollection<Kind> Kinds
        {
            get
            {
                return _Kinds;
            }
            set
            {
                _Kinds = value;
                OnPropertyChanged("Kinds");
            }
        }
        public ObservableCollection<Models.Type> Types
        {
            get
            {
                return _Types;
            }
            set
            {
                _Types = value;
                OnPropertyChanged("Types");
            }
        }
        public Models.Type SelectedType
        {
            get { return selectedType; }
            set
            {
                selectedType = value;
                if(value != null)
                {
                    Kind = value.Kind;
                }
                OnPropertyChanged("SelectedType");
            }
        }
        public Kind Kind
        {
            get
            {
                return _Kind;
            }
            set
            {
                _Kind = value;
                OnPropertyChanged("Kind");
            }
        }
        public string Number
        {
            get
            {
                return _Number;
            }
            set
            {
                _Number = value;
                OnPropertyChanged("Number");
            }
        }
        public DateTime DateOfExpire
        {
            get
            {
                return _DateOfExpire;
            }
            set
            {
                _DateOfExpire = value;
                OnPropertyChanged("DateOfExpire");
            }
        }
        #endregion
        #region Приватные методы модели
        /// <summary>
        /// Получение записей из таблицы Типы документов в коллекцию
        /// </summary>
        private void GetData()
        {
            Types = new ObservableCollection<Models.Type>();
            using (documentContext context = new documentContext())
            {
                List<Models.Type> temp = context.Type.Include(p=>p.Kind).ToList();
                foreach (var item in temp)
                {
                    Types.Add(item);
                }
            }
        }
        /// <summary>
        /// Очистка полей
        /// </summary>
        private void Clear()
        {
            Kind = null;
            Number = "";
            DateOfExpire = DateTime.Today;
            SelectedType = null;
        }
        /// <summary>
        /// Получение данных из таблицы Виды документов в коллекцию
        /// </summary>
        private void GetKinds()
        {
            Kinds = new ObservableCollection<Kind>();
            using (documentContext context = new documentContext())
            {
                List<Kind> temp = context.Kind.ToList();
                foreach (var item in temp)
                {
                    Kinds.Add(item);
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
                if (context.Documents
                    .Where(p => p.IddocumentNavigation.Kind == SelectedType.Kind && p.IddocumentNavigation.Number == SelectedType.Number)
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
