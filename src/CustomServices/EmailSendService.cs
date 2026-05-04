using Kmums.Areas.Identity.Data;
using Kmums.Models;
using Kmums.Models.Contact;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Kmums.CustomServices
{
    public class EmailSendService : IEmailSender
    {
        public EmailAuth EmailAuthOption { get; }
        private readonly DataContext _Dcontext;

        public EmailSendService(IOptions<EmailAuth> EmailAuthOption, DataContext Dcontext)
        {
            this.EmailAuthOption = EmailAuthOption.Value;
            _Dcontext = Dcontext;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(EmailAuthOption.SendgridKey))
            {
                throw new Exception("Null SendGridKey");
            }
           await Execute(EmailAuthOption.SendgridKey, subject, htmlMessage, email);
        }
        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            SendGridClient client = new(apiKey);
            SendGridMessage msg = new()
            {
                From = new EmailAddress("no-reply@kilimanimums.ke","KilimanimumsKe"),
                //ReplyTo = new EmailAddress("info@kilimanimums.ke", "KilimanimumsKe"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(toEmail, "KilimanimumsKe"));

            

            //save sent email to database
            ContactModel Savemsg = new()
            {
                Sender=msg.From.Email,
                Receiver=toEmail,
                Title=msg.Subject,
                Body=message,
                Sent=DateTime.Now.ToString()
            };
            await _Dcontext.AddAsync(Savemsg);
            await _Dcontext.SaveChangesAsync();

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            // msg.SetClickTracking(false, false);

            await client.SendEmailAsync(msg);

        }
    }
}
