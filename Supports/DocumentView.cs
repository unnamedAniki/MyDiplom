using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using System.Windows.Documents;
using Paragraph = DocumentFormat.OpenXml.Wordprocessing.Paragraph;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using System.Windows.Media;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using AccoutingDocs.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Run = DocumentFormat.OpenXml.Wordprocessing.Run;
using DocumentFormat.OpenXml;
using AccoutingDocs.ViewModels;
using System.Threading.Tasks;

namespace AccoutingDocs.Supports
{
    public class DocumentView
    {
        public DocumentView()
        {

        }
        private static string FileName = "";
        public static string ReportPath = Environment.CurrentDirectory;
        /// <summary>
        /// Возвращает имя документа
        /// </summary>
        /// <returns></returns>
        public static string GetName()
        {
            return FileName;
        }
        /// <summary>
        /// Добавляет в компонент просмотра документа созданный документ
        /// </summary>
        /// <returns>Возвращает документ</returns>
        public static FlowDocument AddDocumentToViewer(string path = null)
        {
            var resultDocument = new FlowDocument();
            var dialogService = new DialogService();
            System.Windows.Documents.Table table = new System.Windows.Documents.Table();
            table.RowGroups.Add(new TableRowGroup());
            table.RowGroups[0].Rows.Add(new System.Windows.Documents.TableRow());
            var titleRow = table.RowGroups[0].Rows[0];
            titleRow.FontSize = 16;
            titleRow.FontWeight = FontWeights.Bold;
            var MainPar = new System.Windows.Documents.Paragraph();
            var ind = 1;
            var count = 0;
            if (path == null)
                path = dialogService.GetFilePath();
            if (path != null)
            {
                FileName = path.Substring(path.LastIndexOf(@"\") + 1);
                var view = WordprocessingDocument.Open(path, true);
                foreach (var item in view.MainDocumentPart.Document.Body.ChildElements)
                {
                    var items = item.ToList();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].LocalName == "r" && count == 0)
                        {
                            table.RowGroups[0].Rows.Add(new System.Windows.Documents.TableRow());
                            System.Windows.Documents.Run run = new System.Windows.Documents.Run();
                            titleRow.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(items[i].InnerText))));
                            resultDocument.Blocks.Add(table);
                        }
                        if (items[i].LocalName == "r" && count != 0)
                        {
                            System.Windows.Documents.Run run = new System.Windows.Documents.Run();
                            run.Text = items[i].InnerText;
                            MainPar.Inlines.Add(run.Text + "\n");
                            resultDocument.Blocks.Add(MainPar);
                        }
                        if (items[i].LocalName == "tr")
                        {
                            table.RowGroups[0].Rows.Add(new System.Windows.Documents.TableRow());
                            System.Windows.Documents.TableRow currentRow = table.RowGroups[0].Rows[ind];
                            currentRow.Foreground = Brushes.Black;
                            if (ind != 1)
                            {
                                currentRow.FontSize = 14;
                                currentRow.Background = Brushes.LightBlue;
                            }
                            if (ind % 2 == 0)
                                currentRow.Background = Brushes.LightBlue;
                            else
                                currentRow.Background = Brushes.LightGreen;
                            var cells = items[i].ChildElements.ToList();
                            titleRow.Cells[0].ColumnSpan = cells.Count;
                            for (int j = 0; j < cells.Count; j++)
                            {
                                currentRow.Cells.Add(new System.Windows.Documents.TableCell(new System.Windows.Documents.Paragraph(new System.Windows.Documents.Run(cells[j].InnerText))));
                            }
                            ind++;
                            resultDocument.Blocks.Add(table);
                        }
                    }
                    count++;
                }
            }
            return resultDocument;
        }
        /// <summary>
        /// Создает отчет об использовании внутренних документов за указанный период
        /// </summary>
        /// <param name="viewer">Вывод созданного документа</param>
        public static void CreateDoc(FlowDocumentScrollViewer viewer, DateTime Begin, DateTime End)
        {
            viewer = null;
            ReportPath = Environment.CurrentDirectory;
            FolderCreate();
            ReportPath += @"\Отчеты" + @"\_Отчет_об_использовании_внутренних_документов_за_" + DateTime.Now.ToShortDateString() + ".docx";
            using (documentContext context = new documentContext())
            {
                List<Register> temp = context.Register
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.User)
                    .Include(p => p.Status)
                    .Where(p => p.Status.Status1 == "Используется" && p.TakenDate.Date >= Begin.Date && p.Document.UsingDate.Value.Date <= End.Date)
                    .ToList();
                if (temp.Count != 0 && NewDoc())
                {
                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(ReportPath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        Table table = new Table();
                        TableProperties tblProp = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 }
                                )
                            );
                        table.AppendChild<TableProperties>(tblProp);
                        var tc1 = new TableCell();
                        var tr = new TableRow();
                        run.AppendChild(new Text("Отчет об использовании входящих документов за период " + Begin.ToShortDateString()+ " - " + End.ToShortDateString()));
                        body.AppendChild(table);
                        tr = new TableRow();
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Дата регистрации"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Номер документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Подразделение"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Содержание документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Ответственное лицо"))));
                        tr.Append(tc1);
                        table.Append(tr);
                        foreach (var item in temp)
                        {
                            tr = new TableRow();
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.TakenDate.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Id.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.User.Staff.Staff_Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.User.Fam + " " + item.User.Name + " " + item.User.Lastname))));
                            tr.Append(tc1);
                            table.Append(tr);
                        }
                        Paragraph para_ = body.AppendChild(new Paragraph());
                        Run run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Дата: " + DateTime.Now.ToShortDateString()));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Секретарь: _____________________(подпись)"));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Расшифровка подписи: " + AuthViewModel.currentUser.User.Fam + " " +
                            AuthViewModel.currentUser.User.Name + " " +
                            AuthViewModel.currentUser.User.Lastname));
                    }
                }
                else
                {
                    MessageBox.Show("За выбранный период внутренних документов нет");
                }
            }
        }
        /// <summary>
        /// Создает отчет об исходящих документов за указанный период
        /// </summary>
        /// <param name="viewer">Вывод созданного документа</param>
        public static void CreateSendingDoc(FlowDocumentScrollViewer viewer, DateTime Begin, DateTime End)
        {
            viewer = null;
            ReportPath = Environment.CurrentDirectory;
            FolderCreate();
            ReportPath += @"\Отчеты" + @"\_Отчет_об_исходящих_документах_за_" + DateTime.Now.ToShortDateString() + ".docx";
            using (documentContext context = new documentContext())
            {
                List<Register> temp = context.Register
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.EditUser)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.User)
                    .Include(p => p.Status)
                    .Where(p => p.Status.Status1 == "Отправлен" && p.TakenDate.Date >= Begin.Date && p.TakenDate.Date <= End.Date)
                    .ToList();
                if (temp.Count != 0 && NewDoc())
                {
                    using (WordprocessingDocument wordDocument = WordprocessingDocument.Create(ReportPath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        Table table = new Table();
                        TableProperties tblProp = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 }
                                )
                            );
                        table.AppendChild<TableProperties>(tblProp);
                        var tc1 = new TableCell();
                        var tr = new TableRow();
                        run.AppendChild(new Text("Отчет о регистрации исходящих документов за период " + Begin.ToShortDateString() + " - " + End.ToShortDateString()));
                        body.AppendChild(table);
                        tr = new TableRow();
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Дата регистрации"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Номер документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Предприятие-получатель"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Электронный адрес предприятия"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Содержание документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Исполнитель"))));
                        tr.Append(tc1);
                        table.Append(tr);
                        foreach (var item in temp)
                        {
                            tr = new TableRow();
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.TakenDate.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Id.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.NameFromNavigation.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.NameFromNavigation.Mail))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.EditUser.Fam + " " + item.Document.EditUser.Name + " " + item.Document.EditUser.Lastname))));
                            tr.Append(tc1);
                            table.Append(tr);
                        }
                        Paragraph para_ = body.AppendChild(new Paragraph());
                        Run run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Дата: " + DateTime.Now.ToShortDateString()));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Секретарь: _____________________(подпись)"));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Расшифровка подписи: " + AuthViewModel.currentUser.User.Fam + " " +
                            AuthViewModel.currentUser.User.Name + " " +
                            AuthViewModel.currentUser.User.Lastname));
                    }
                }
                else
                {
                    MessageBox.Show("За выбранный период исходящих документов нет");
                }
            }
        }
        /// <summary>
        /// Проверка наличия уже созданного отчета
        /// </summary>
        private static bool NewDoc()
        {
            if (File.Exists(ReportPath))
            {
                try
                {
                    File.Delete(ReportPath);
                    return true;
                }
                catch 
                {
                    MessageBox.Show("Закройте все программы MS Word и повторите попытку");
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Создает Отчет об использовании внутренних документов на указанном подразделении
        /// </summary>
        /// <param name="viewer">Вывод созданного документа</param>
        public static bool CreateStaffReport(FlowDocumentScrollViewer viewer, Staff staff)
        {
            using (documentContext context = new documentContext())
            {
                List<Staffdocuments> temp = context.Staffdocuments
                    .Include(p => p.HeadUser)
                    .ThenInclude(p => p.Staff)
                    .Include(p=>p.Document)
                    .ThenInclude(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.NameFromNavigation)
                    .Include(p => p.Document)
                    .ThenInclude(p => p.Status)
                    .Where(p => p.HeadUser.Staff == staff)
                    .ToList();
                    viewer = null;
                    ReportPath = Environment.CurrentDirectory;
                    FolderCreate();
                ReportPath += @"\Отчеты" + @"\_Отчет_за_" + DateTime.Now.ToShortDateString() + "_период_в_подразделении_" + staff.Staff_Name + ".docx";
                if (temp.Count != 0 && NewDoc())
                {
                    using (WordprocessingDocument wordDocument =
                           WordprocessingDocument.Create(ReportPath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        Table table = new Table();
                        TableProperties tblProp = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 }
                                )
                            );
                        table.AppendChild<TableProperties>(tblProp);
                        var tc1 = new TableCell();
                        var tr = new TableRow();
                        run.AppendChild(new Text("Отчет об использовании входящих документов в подразделении  " + staff.Staff_Name));
                        body.AppendChild(table);
                        tr = new TableRow();
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Дата получения"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Номер документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Содержание документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Наименование подразделения"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Руководитель подразделения"))));
                        tr.Append(tc1);
                        table.Append(tr);
                        foreach (var item in temp)
                        {
                            tr = new TableRow();
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.EndingDate.AddDays(-7).ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Id.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Document.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.HeadUser.Staff.Staff_Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.HeadUser.Fam + " " + item.HeadUser.Name + " " + item.HeadUser.Lastname))));
                            tr.Append(tc1);
                            table.Append(tr);
                        }
                        Paragraph para_ = body.AppendChild(new Paragraph());
                        Run run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Дата: " + DateTime.Now.ToShortDateString()));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Секретарь: _____________________(подпись)"));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Расшифровка подписи: " + AuthViewModel.currentUser.User.Fam + " " +
                            AuthViewModel.currentUser.User.Name + " " +
                            AuthViewModel.currentUser.User.Lastname));
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("На выбранном предприятии используемых документов нет");
                    return false;
                }
            }
        }
        /// <summary>
        /// Создает Отчет о поступлении документов за указанный период
        /// </summary>
        /// <param name="viewer">Вывод созданного документа</param>
        public static bool CreatePeriodReport(FlowDocumentScrollViewer viewer, DateTime Begin, DateTime End)
        {
            using (documentContext context = new documentContext())
            {
                List<Documents> temp = context.Documents
                    .Include(p => p.User)
                    .ThenInclude(p => p.Staff)
                    .Include(p => p.IddocumentNavigation)
                    .ThenInclude(p => p.Kind)
                    .Include(p => p.NameFromNavigation)
                    .Include(p => p.Status)
                    .Where(p => p.AddingDate.Date >= Begin.Date && p.AddingDate.Date <= End.Date && p.Status.Status1 != "Отправлен")
                    .ToList();
                    viewer = null;
                    ReportPath = Environment.CurrentDirectory;
                    FolderCreate();
                    ReportPath += @"\Отчеты" + @"\_Отчет_по_поступившим_документам_на_" + Begin.ToShortDateString() +"-"+End.ToShortDateString()+ "_период_за_"+DateTime.Now.ToShortDateString()+".docx";
                if (temp.Count != 0 && NewDoc())
                {
                    using (WordprocessingDocument wordDocument =
                           WordprocessingDocument.Create(ReportPath, WordprocessingDocumentType.Document))
                    {
                        MainDocumentPart mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        Body body = mainPart.Document.AppendChild(new Body());
                        Paragraph para = body.AppendChild(new Paragraph());
                        Run run = para.AppendChild(new Run());
                        Table table = new Table();
                        TableProperties tblProp = new TableProperties(
                            new TableBorders(
                                new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 },
                                new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.BasicThinLines), Size = 5 }
                                )
                            );
                        table.AppendChild<TableProperties>(tblProp);
                        var tc1 = new TableCell();
                        var tr = new TableRow();
                        run.AppendChild(new Text("Отчет о регистрации входящих документов за период с " + Begin.ToShortDateString() + " по " + End.ToShortDateString()));
                        body.AppendChild(table);
                        tr = new TableRow();
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Дата поступления"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Номер регистрации"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Номер и дата исходящего"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Содержание документа"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Организация-отправитель"))));
                        tr.Append(tc1);
                        tc1 = new TableCell();
                        tc1.Append(new Paragraph(new Run(new Text("Ответственное лицо"))));
                        tr.Append(tc1);
                        table.Append(tr);
                        foreach (var item in temp)
                        {
                            tr = new TableRow();
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.AddingDate.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Id.ToString()))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Commend))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.NameFromNavigation.Name))));
                            tr.Append(tc1);
                            tc1 = new TableCell();
                            tc1.Append(new Paragraph(new Run(new Text(item.User.Fam + " " + item.User.Name + " " + item.User.Lastname))));
                            tr.Append(tc1);
                            table.Append(tr);
                        }
                        Paragraph para_ = body.AppendChild(new Paragraph());
                        Run run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Дата: " + DateTime.Now.ToShortDateString()));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Секретарь: _____________________(подпись)"));
                        para_ = body.AppendChild(new Paragraph());
                        run_ = para_.AppendChild(new Run());
                        run_.AppendChild(new Text("Расшифровка подписи: " + AuthViewModel.currentUser.User.Fam + " " +
                            AuthViewModel.currentUser.User.Name + " " +
                            AuthViewModel.currentUser.User.Lastname));
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("За выбранный период поступивших документов нет");
                    return false;
                }
            }
        }
        /// <summary>
        /// Созданиее папки для отчетов
        /// </summary>
        private static void FolderCreate()
        {
            if (!Directory.Exists(ReportPath + @"\Отчеты"))
            {
                Directory.CreateDirectory(ReportPath + @"\Отчеты");
            }
        }
    }
}
