using Data.Models;
using System.Threading.Tasks;

namespace Business
{
    public interface IEmailSender
    {
        public Task SendEmailAsync(string sender,
                                   string sednerName,
                                   string reciever,
                                   string subject,
                                   string htmlMessage);

        public Task SendEmailAsync(string reciever,
                                   string subject,
                                   string htmlMessage);

        public Task SendTicketCancelationEmailAsync(string reciever,
                                                    FilmProjection projection,
                                                    string projectionsUrlPattern);

        public Task SendTicketUpdateEmailAsync(string reciever,
                                                     FilmProjection projection,
                                                     string ticketsUrlPattern);

        public Task SendEmailConfirmationEmailAsync(string reciever, string callbackUrl);

    }
}
