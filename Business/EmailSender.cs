using Data.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Business
{
    public class EmailSender : IEmailSender
    {
        private readonly SendGridClient client;
        private readonly string sender;
        private readonly string senderName;

        public EmailSender(string APIKey, string sender, string senderName)
        {
            this.client = new SendGridClient(APIKey);
            this.sender = sender;
            this.senderName = senderName;
        }

        /// <summary>
        /// Sends email from a selected sender to a selected receiver with subject and message
        /// </summary>
        /// <returns></returns>
        public async Task SendEmailAsync(string sender,
                                         string senderName,
                                         string reciever,
                                         string subject,
                                         string htmlMessage)
        {
            var from = new EmailAddress(sender, senderName);
            var to = new EmailAddress(reciever);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Sends email from a default sender to a selected receiver with subject and message
        /// </summary>
        /// <returns></returns>
        public async Task SendEmailAsync(string reciever,
                                         string subject,
                                         string htmlMessage)
        {
            var from = new EmailAddress(sender, senderName);
            var to = new EmailAddress(reciever);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlMessage);
            await client.SendEmailAsync(msg);
        }

        /// <summary>
        /// Sends email to cancel a ticket reservation for a selected projection
        /// </summary>
        /// <returns></returns>
        public async Task SendTicketCancelationEmailAsync(string reciever, 
                                                          FilmProjection projection,
                                                          string projectionsUrlPattern)
        {
            var subject = "Ticket cancellation alert";
            projectionsUrlPattern = HtmlEncoder.Default.Encode(projectionsUrlPattern);
            var html = $"<p style=\"color: #000\">Regret to inform you that your ticket for the {projection.Film.FilmData.Title}" +
                $" in cinema \"{projection.Cinema.Name}\" on {projection.Date.ToString("dd/MM/yyyy HH/mm")} has been canceled" +
                $" due to projection cancelation.</p> " +
                $"</br> <p> You can go and reserve a new one for a projection you'd like by clicking" +
                $" <a href=\"{projectionsUrlPattern}\">here</a></p>";
            await SendEmailAsync(reciever, subject, html);
        }

        /// <summary>
        /// Sends email to inform about ticket information update
        /// </summary>
        /// <returns></returns>
        public async Task SendTicketUpdateEmailAsync(string reciever,
                                                     FilmProjection projection, 
                                                     string ticketsUrlPattern)
        {
            ticketsUrlPattern = HtmlEncoder.Default.Encode(ticketsUrlPattern);
            var subject = "Ticket update alert";
            var html = $"<p style=\"color: #000\">We inform you that your ticket for the {projection.Film.FilmData.Title}" +
            $" in cinema \"{projection.Cinema.Name}\" on {projection.Date.ToString("dd/MM/yyyy HH/mm")} has been updated" +
            $" due to projection changes.</p> </br> " +
            $"<p> You can go and see your new ticket or you can cancel it by clicking " +
            $"<a href=\"{ticketsUrlPattern}\">here</a></p>";
            await SendEmailAsync(reciever, subject, html);
        }

        /// <summary>
        /// Sends a confirmation email to a receiver from a default sender
        /// </summary>
        /// <returns></returns>
        public async Task SendEmailConfirmationEmailAsync(string reciever, string callbackUrl)
        {
            var subject= "Confirm your email";
            callbackUrl = HtmlEncoder.Default.Encode(callbackUrl);
            var html = $"<p style=\"color: #000\">Please confirm your account by" +
                $" <a style=\"text-decoration: none !important\" href='{callbackUrl}'>clicking here</a>.</p>";
            await SendEmailAsync(reciever, subject, html);
        }

    }
}
