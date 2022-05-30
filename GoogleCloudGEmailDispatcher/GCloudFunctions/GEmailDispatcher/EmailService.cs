using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;
using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using static Google.Apis.Gmail.v1.UsersResource.MessagesResource;

namespace EmailDispatcher
{
    public class EmailService
    {
        private static readonly string ApplicationName = "EmailDispatcher";

        private readonly GmailService gmailService;

        public EmailService(UserCredential credential)
        {
            gmailService = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<bool> SendEmailAsync(Email emailData)
        {
            var gEmail = ConfigureMail(emailData);

            SendRequest request = gmailService.Users.Messages.Send(gEmail, "me");
            var result = await request.ExecuteAsync();

            return await WasEmailSentSuccessfullyAsync(result.ThreadId);
        }

        private static Message ConfigureMail(Email emailData)
        {
            var mail = new MailMessage(emailData.From, emailData.To)
            {
                Subject = emailData.Subject,
                Body = emailData.Content
            };

            if (!string.IsNullOrEmpty(emailData.CC))
                mail.CC.Add(emailData.CC);
            if (!string.IsNullOrEmpty(emailData.BCC))
                mail.Bcc.Add(emailData.BCC);

            if (!string.IsNullOrEmpty(emailData.ReplyTo))
                mail.ReplyToList.Add(emailData.ReplyTo);
            else
                mail.ReplyToList.Add(emailData.From);

            mail.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            var mimeMail = MimeMessage.CreateFromMailMessage(mail);
            return new Message { Raw = EncodeMail(mimeMail) };
        }

        private async Task<bool> WasEmailSentSuccessfullyAsync(string threadId)
        {
            var queryInbox = gmailService.Users.Messages.List("me");
            queryInbox.Q = "from:mailer-daemon@googlemail.com";

            var inboxEmailsResult = await queryInbox.ExecuteAsync();

            return !inboxEmailsResult.Messages.Any(m => m.ThreadId == threadId);
        }

        private static string EncodeMail(MimeMessage message)
        {
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);

                return Convert.ToBase64String(stream.GetBuffer())
                              .TrimEnd('=')
                              .Replace('+', '-')
                              .Replace('/', '_');
            }
        }
    }
}