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
    public class StaffdocumentsViewModel : INotifyPropertyChanged
    {
        public StaffdocumentsViewModel()
        {
            GetData();
        }
        Staffdocuments newdoc = new Staffdocuments();
        #region Команды ViewModel
        /// <summary>
        /// Открытие диалогового окна
        /// </summary>
        public ICommand OpenConfirm
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    DialogHost.OpenDialogCommand.Execute(null, null);
                }, obj => SelectedDoc != null);
            }
        }
        /// <summary>
        /// Подтверждение выполнения работы с документом
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
                        temp.Status = context.Documents.Where(p => p.Id == SelectedDoc.DocumentId).FirstOrDefault().Status = context.Status.FirstOrDefault(p => p.Status1 == "Возвращен в базу данных");
                        context.Documents.Update(temp);
                        context.SaveChanges();
                    }
                    SelectedDoc = null;
                    DialogHost.CloseDialogCommand.Execute(null, null);
                }, obj => SelectedDoc != null);
            }
        }
        #endregion
        #region Приватные свойства
        private Users _HeadUser { get; set; }
        private Staffdocuments _SelectedDoc { get; set; }
        private ObservableCollection<Staffdocuments> _AllDocuments { get; set; }
        #endregion
        #region Публичные свойства
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
