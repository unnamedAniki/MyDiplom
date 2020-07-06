using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using AccoutingDocs.Models;
using AccoutingDocs.Commands;
using AccoutingDocs.Supports;
using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.ObjectModel;
using System;
using Microsoft.Office.Interop.Word;
using System.Windows;

namespace AccoutingDocs.ViewModels
{
    public class ReportViewModel : INotifyPropertyChanged
    {
        public ReportViewModel()
        {
            BeginingDate = DateTime.Now;
            EndingDate = DateTime.Now;
            Name = "*тут будет наименование сформированного документа*";
            Staffs = new ObservableCollection<Staff>();
            using (documentContext context = new documentContext())
            {
                List<Staff> collection = context.Staff.ToList();
                foreach (var item in collection)
                {
                    Staffs.Add(item);
                }
            }
        }
        #region Команда открытия диалогового окна
        /// <summary>
        /// Открыть диалоговое окно
        /// </summary>
        public ICommand OpenDialog
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    flow = (FlowDocumentScrollViewer)obj;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                });
            }
        }
        #endregion
        #region Команды для документов
        /// <summary>
        /// Получить путь файла
        /// </summary>
        public ICommand GetFilePath 
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    var doc = (FlowDocumentScrollViewer)obj;
                    doc.Document = DocumentView.AddDocumentToViewer();
                    Name = DocumentView.GetName();
                });
            }
        }
        /// <summary>
        /// Создать отчет об использовании внутренних документов за период
        /// </summary>
        public ICommand CreateRegisterUsing
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    flow = (FlowDocumentScrollViewer)obj;
                    try
                    {
                        DocumentView.CreateDoc(flow, BeginingDate, EndingDate);
                        flow.Document = DocumentView.AddDocumentToViewer(DocumentView.ReportPath);
                    }
                    catch { }
                }, (obj) => BeginingDate < EndingDate);
            }
        }
        /// <summary>
        /// Создать отчет об исходящих документах за период
        /// </summary>
        public ICommand CreateSendingRegister
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    flow = (FlowDocumentScrollViewer)obj;
                    try
                    {
                        DocumentView.CreateSendingDoc(flow, BeginingDate, EndingDate);
                        flow.Document = DocumentView.AddDocumentToViewer(DocumentView.ReportPath);
                    }
                    catch { }
                }, (obj) => BeginingDate < EndingDate);
            }
        }
        /// <summary>
        /// Создать отчет по поступившим документам за период
        /// </summary>
        public ICommand CreatePeriodReport
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    flow = (FlowDocumentScrollViewer)obj;
                    try
                    {
                        if (DocumentView.CreatePeriodReport(flow, BeginingDate, EndingDate))
                        {
                            flow.Document = DocumentView.AddDocumentToViewer(DocumentView.ReportPath);
                            Name = DocumentView.GetName();
                        }
                    }
                    catch(Exception e) 
                    {
                        MessageBox.Show(e.ToString());
                    }
                }, (obj) => BeginingDate < EndingDate);
            }
        }
        /// <summary>
        /// Создать отчет об использовании внутренних документов на предприятии
        /// </summary>
        public ICommand CreateStaffDocument
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    if (DocumentView.CreateStaffReport(flow, SelectedStaff))
                    {
                        flow.Document = DocumentView.AddDocumentToViewer(DocumentView.ReportPath);
                        Name = DocumentView.GetName();
                    }
                    DialogHost.CloseDialogCommand.Execute(null, null);
                });
            }
        }
        /// <summary>
        /// Открыть диалоговое окно выбора подразделений
        /// </summary>
        public ICommand OpenCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    flow = (FlowDocumentScrollViewer)obj;
                    DialogHost.OpenDialogCommand.Execute(null, null);
                });
            }
        }
        /// <summary>
        /// Открыть созданный отчет в MS Word
        /// </summary>
        public ICommand OpenReportCommand
        {
            get
            {
                return new DelegateCommand(obj =>
                {
                    ap = new Microsoft.Office.Interop.Word.Application();
                    try
                    {
                        ap.Documents.Open(DocumentView.ReportPath);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Закройте все программы MS Word и попробуйте открыть еще раз", "Ошибка открытия файла! ");
                    }
                    ap.Visible = true;
                    flow = null;
                }, (obj) => flow != null);
            }
        }
        #endregion
        #region Свойства модели
        private Microsoft.Office.Interop.Word.Application ap { get; set; }
        private FlowDocumentScrollViewer flow { get; set; }
        public ObservableCollection<Staff> Staffs { get; set; }
        private Staff selectedStaff;
        public Staff SelectedStaff
        {
            get { return selectedStaff; }
            set
            {
                selectedStaff = value;
                OnPropertyChanged("SelectedStaff");
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
        private DateTime _BeginingDate { get; set; }
        public DateTime BeginingDate
        {
            get { return _BeginingDate; }
            set
            {
                _BeginingDate = value;
                OnPropertyChanged("BeginingDate");
            }
        }
        private DateTime _EndingDate { get; set; }
        public DateTime EndingDate
        {
            get { return _EndingDate; }
            set
            {
                _EndingDate = value;
                OnPropertyChanged("EndingDate");
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
