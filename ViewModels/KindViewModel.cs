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
    public class KindViewModel : INotifyPropertyChanged
    {
        public KindViewModel()
        {
            GetKind();
        }
        public ObservableCollection<Kind> _kindnames { get; set; }
        public ObservableCollection<Kind> kindnames
        {
            get { return _kindnames; }
            set
            {
                _kindnames = value;
                OnPropertyChanged("kindnames");
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
                    SelectedKind = null;
                    using (documentContext context = new documentContext())
                    {
                        if (!context.Kind.Where(p => p.KindName == KindName).Any())
                        {
                            var temp = new Kind();
                            temp.KindName = KindName;
                            context.Kind.Add(temp);
                            context.SaveChanges();
                        }
                        else
                        {
                            MessageBox.Show("Данный вид документа уже есть");
                        }
                    }
                    GetKind();
                    KindName = "";
                    SelectedKind = null;
                }, (obj) => KindName != "");
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
                    if (SelectedKind != null)
                    {
                        using (documentContext context = new documentContext())
                        {
                            var temp = context.Kind.Find(SelectedKind.Id);
                            temp.KindName = KindName;
                            if (!context.Kind.Where(p => p.KindName == KindName && p.Id != temp.Id).Any())
                            {
                                context.Kind.Update(temp);
                                context.SaveChanges();
                            }
                            else
                            {
                                MessageBox.Show("Данный вид документа уже есть!", "Ошибка!");
                            }
                        }
                        GetKind();
                        KindName = "";
                        SelectedKind = null;
                    }
                }, (obj) => SelectedKind != null && KindName != "");
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
                    if (MessageBox.Show("Вы действительно хотите удалить данный вид документа?", "Внимание!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        if (!ValidateKind())
                        {
                            using (documentContext context = new documentContext())
                            {
                                var removingKind = context.Kind.Where(p => p.KindName == SelectedKind.KindName).FirstOrDefault();
                                context.Kind.Remove(removingKind);
                                context.SaveChanges();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Данный вид документа уже используется! Удаление невозможно!", "Ошибка!");
                        }
                    }
                    GetKind();
                    KindName = "";
                    SelectedKind = null;

                }, (obj) => SelectedKind != null);
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
                        GetKind();
                    }
                });
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
        private string _KindName { get; set; }
        public string KindName
        {
            get { return _KindName; }
            set
            {
                _KindName = value;
                OnPropertyChanged("KindName");
            }
        }
        private Kind selectedKind;
        public Kind SelectedKind
        {
            get { return selectedKind; }
            set
            {
                selectedKind = value; ;
                OnPropertyChanged("SelectedKind");
            }
        }
        /// <summary>
        /// Загружает данные из таблицы Виды документов в коллекцию
        /// </summary>
        private void GetKind()
        {
            kindnames = new ObservableCollection<Kind>();
            using (documentContext context = new documentContext())
            {
                List<Kind> temp = context.Kind.ToList();
                foreach (var item in temp)
                {
                    kindnames.Add(item);
                }
            }
        }
        /// <summary>
        /// Валидация
        /// </summary>
        private bool ValidateKind()
        {
            using (documentContext context = new documentContext())
            {
                if (context.Type.Where(p => p.KindId == SelectedKind.Id).Any())
                    return true;
                return false;
            }
        }
        /// <summary>
        /// Получения результата поиска
        /// </summary>
        private void GetSearchResult(string text)
        {
            var temp = kindnames.Where(p => p.KindName == text).ToList();
            kindnames = new ObservableCollection<Kind>();
            foreach (var item in temp)
            {
                kindnames.Add(item);
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
