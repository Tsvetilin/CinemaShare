using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridClient client;


        public EmailSender(string APIKey)
        {
            this.client = new SendGridClient(APIKey);
        }

        public async Task SendEmailAsync(string sender,
                                   string senderName,
                                   string reciever,
                                   string subject,
                                   string htmlMessage)
        {
            var from = new EmailAddress(sender, senderName);
            var to = new EmailAddress(reciever);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null,htmlMessage);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
