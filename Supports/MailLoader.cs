using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using AccoutingDocs.ViewModels;
using MailKit;
using MailKit.Net.Imap;
using MimeKit;

namespace AccoutingDocs.Supports
{
    public class MailLoader
    {
        /// <summary>
        /// Добавление в таблицу всех писем с почтового адресса
        /// </summary>
        /// <param name="dataGrid">Таблица для вывода</param>
        public async void Receive(DataGrid dataGrid)
        {
            var mailTask = Task.Run(async() => {
                try
                {
                    if (dataGrid.ItemsSource == null)
                    {
                        using (var client = new ImapClient())
                        {
                            client.ServerCertificateValidationCallback = (s, c, h, e) => true;
                            await client.ConnectAsync("imap.mail.me.com", 993, true);
                            await client.AuthenticateAsync("karavka", "xgkr-xrop-mkfa-rvkg");
                            var inbox = client.Inbox;
                            inbox.Open(FolderAccess.ReadOnly);
                            List<Mail.Mails> mail = new List<Mail.Mails>();
                            var attachMail = await inbox.FetchAsync(0, -1, MessageSummaryItems.UniqueId | MessageSummaryItems.BodyStructure);
                            var AttachMail = attachMail.Where(p => p.Attachments.Any(p =>
                                p.FileName.EndsWith(".doc") ||
                                p.FileName.EndsWith(".pdf") ||
                                p.FileName.EndsWith(".csv") ||
                                p.FileName.EndsWith(".docx")));
                            foreach (var i in AttachMail)
                            {
                                var message = await inbox.GetMessageAsync(i.Index);
                                var attach = message.Attachments.FirstOrDefault();
                                var mails = message.From.Mailboxes.FirstOrDefault().Address;
                                var fileName = "Документы/" + attach.ContentDisposition?.FileName ?? attach.ContentType.Name;
                                mail.Add(new Mail.Mails
                                {
                                    date = message.Date,
                                    From = mails,
                                    Subject = message.Subject,
                                    FileName = attach.ContentDisposition?.FileName ?? attach.ContentType.Name,
                                    Attach = fileName,
                                });
                                if (!Directory.Exists("Документы"))
                                    Directory.CreateDirectory("Документы");
                                if (!File.Exists(fileName))
                                {
                                    using (var stream = File.Create(fileName))
                                    {
                                        if (attach is MessagePart)
                                        {
                                            var rfc822 = (MessagePart)attach;

                                            await rfc822.Message.WriteToAsync(stream);
                                        }
                                        else
                                        {
                                            var part = (MimePart)attach;

                                            await part.Content.DecodeToAsync(stream);
                                        }
                                    }
                                }
                                App.MainMenuDispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)async delegate ()
                                {
                                    dataGrid.ItemsSource = mail.ToList();
                                });
                            }
                            client.Disconnect(true);
                        }
                    }
                }
                catch
                {
                    return;
                }
            });
            await mailTask.ContinueWith(obj => 
            { 
                if(obj.Status == TaskStatus.RanToCompletion)
                {
                    App.MainMenuDispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)async delegate () 
                    {
                        App.model.Value = 100;
                        App.model.Result = "Загрузка завершена";
                    });
                };
            });
        }
    }
}
