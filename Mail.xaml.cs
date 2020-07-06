using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using MailKit.Security;
using System.IO;

namespace AccoutingDocs
{
    /// <summary>
    /// Логика взаимодействия для Mails.xaml
    /// </summary>
    public partial class Mail : UserControl
    {
        public class Mails
        {
            public DateTimeOffset date { get; set; }
            public string Subject { get; set; }
            public string From { get; set; }
            public dynamic FileName { get; set; }
            public dynamic Attach { get; set; }
        }
        public Mail()
        {
            InitializeComponent();
            try
            {
                Supports.MailLoader mailLoader = new Supports.MailLoader();
                mailLoader.Receive(Mail_);
            }
            catch (Exception ex)
            {
                MessageBox.Show("О нет! Что-то пошло не так!\n" + "Содержание ошибки:\n" + ex, "Ошибка");
            }
        }
    }
}
