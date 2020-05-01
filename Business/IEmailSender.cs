using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string sender, string sednerName, string reciever, string subject, string htmlMessage);
    }
}
