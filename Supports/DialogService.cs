using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using AccoutingDocs.Interfaces;
using Microsoft.Win32;

namespace AccoutingDocs.Supports
{
    class DialogService : IDialogService
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// Получить имя файла
        /// </summary>
        /// <returns>Возращает строку с наименованием файла</returns>
        public string GetFileName()
        {
            if (FileName == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Office Files |*.doc;*.xls;*.docx|PDF files |*.PDF;";
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath = openFileDialog.FileName;
                    FileName = FilePath.Substring(FilePath.LastIndexOf(@"\") + 1);
                    FileName = FileName.Substring(0, FileName.LastIndexOf("."));
                    return FileName;
                }
                return null;
            }
            else
            {
                return FileName;
            }
        }
        /// <summary>
        /// Получить путь к файлу
        /// </summary>
        /// <returns>Возращает строку с путем файла</returns>
        public string GetFilePath()
        {
            if (FilePath == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Office Files |*.doc;*.xls;*.docx|PDF files |*.PDF;";
                if (openFileDialog.ShowDialog() == true)
                {
                    FilePath = openFileDialog.FileName;
                    FileName = FilePath.Substring(FilePath.LastIndexOf(@"\") + 1);
                    return FilePath;
                }
                return null;
            }
            else
            {
                return FilePath;
            }
        }
    }
}
