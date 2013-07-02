using FeedCollector.Core.Models;
using FeedCollector.Core.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Net;
using System.Net.Mail;
using System.Xml;

namespace FeedCollector.Services.Handlers
{
    [Export(typeof(HandlerServiceBase))]
    public class EmailReportHandler : HandlerServiceBase
    {
        private string _host, _username, _password, _from, _to, _subject;

        public override void Process(IEnumerable<Content> items)
        {
            LoadConfigData();

            using (var smtp = new SmtpClient(this._host))
            {
                var body = items.Any() ?
                    String.Join("<br /><br />", items.Select(i => String.Format("{0} - {1:MM/dd/yyyy} - {2}", i.Site, i.Date, i.Title)))
                    : "No items processed today.";

                var msg = new MailMessage(this._from, this._to)
                {
                    Subject = this._subject ?? "Daily Download Report",
                    IsBodyHtml = true,
                    Body = body
                };

                smtp.Credentials = new NetworkCredential(this._username, this._password);
                smtp.Send(msg);
            }
        }

        private void LoadConfigData()
        {
            var configFile = "EmailReportHandler.config";

            var document = new XmlDocument();
            document.Load(configFile);

            var smtpNode = document.SelectSingleNode("smtp");

            this._host = smtpNode.SelectSingleNode("host").InnerText;
            this._username = smtpNode.SelectSingleNode("username").InnerText;
            this._password = smtpNode.SelectSingleNode("password").InnerText;
            this._from = smtpNode.SelectSingleNode("from").InnerText;
            this._to = smtpNode.SelectSingleNode("to").InnerText;
            this._subject = smtpNode.SelectSingleNode("subject").InnerText;
        }
    }
}
