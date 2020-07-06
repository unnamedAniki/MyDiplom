using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using AccoutingDocs.Models;

namespace AccoutingDocs.ViewModels
{
    public class DictViewModel : INotifyPropertyChanged
    {
        public DictViewModel()
        {
            Dicts = MainMenu.GetDict();
        }
        private MainMenu _SelectedDict;
        public MainMenu SelectedDict
        {
            get
            {
                return _SelectedDict;
            }
            set
            {
                _SelectedDict = value;
                OnPropertyChanged("SelectedDict");
            }
        }
        private ObservableCollection<MainMenu> _Dicts;
        public ObservableCollection<MainMenu> Dicts
        {
            get
            {
                return _Dicts;
            }
            set
            {
                _Dicts = value;
                OnPropertyChanged("Dicts");
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
