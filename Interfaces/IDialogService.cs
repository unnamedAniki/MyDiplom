using System;
using System.Collections.Generic;
using System.Text;

namespace AccoutingDocs.Interfaces
{
    public interface IDialogService
    {
        string FilePath { get; set; }
        string FileName { get; set; }
        string GetFileName();
        string GetFilePath();
    }
}
