using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Weikio.ApiFramework.Plugins.Email
{
    public class EmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private HashSet<string> _allowedEmails { get; set; }
        private List<string> _allowedDomains { get; set; }

        public EmailOptions Configuration { get; set; }

        public EmailSender(ILogger<EmailSender> logger)
        {
            _logger = logger;
        }

        public async Task<bool> Create(Email email)
        {
            _allowedEmails = new HashSet<string>((Configuration.AllowedReceivers ?? new HashSet<string>()).Where(address => !address.StartsWith("@")));
            _allowedDomains = (Configuration.AllowedReceivers ?? new HashSet<string>()).Where(address => address.StartsWith("@")).ToList();

            using (var msg = CreateMailMessage(email.EmailAddress, email.Subject, email.Message, email.HtmlMessage, false, email.Attachments))
            {
                var result = await SendEmail(msg);
            
                return result;
            }
        }

        private async Task<bool> SendEmail(MailMessage msg)
        {
            try
            {
                RemoveUnallowedReceivers(msg);

                if (msg.To.Any())
                {
                    using (var client = CreateClient())
                    {
                        await client.SendMailAsync(msg);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send email {Msg}", msg);

                return false;
            }
        }

        private void RemoveUnallowedReceivers(MailMessage msg)
        {
            foreach (var address in msg.To.ToList())
            {
                if (!IsAllowedReceiver(address))
                {
                    msg.To.Remove(address);
                }
            }
        }

        private bool IsAllowedReceiver(MailAddress address)
        {
            // Handle case where there isn't any email addresses in 
            if (Configuration.AllowedReceivers?.Any() != true)
            {
                return true;
            }

            if (_allowedEmails.Any(allowed => string.Compare(allowed, address.Address, StringComparison.InvariantCultureIgnoreCase) == 0))
            {
                return true;
            }

            if (_allowedDomains.Any(domain => address.Address.EndsWith(domain, StringComparison.InvariantCultureIgnoreCase)))
            {
                return true;
            }

            return false;
        }

        private MailMessage CreateMailMessage(string email, string subject, string message, string htmlMessage, bool useDefaultHeaders = true,
            List<EmailAttachment> emailAttachment = null)
        {
            var isBodyHtml = false;

            if (!string.IsNullOrWhiteSpace(htmlMessage))
            {
                message = useDefaultHeaders
                    ? Configuration.DefaultHeaderHtml + htmlMessage + Configuration.DefaultFooterHtml
                    : htmlMessage;

                isBodyHtml = true;
            }
            else
            {
                if (useDefaultHeaders)
                {
                    message = Configuration.DefaultHeaderText + message + Configuration.DefaultFooterText;
                }
            }

            var result = new MailMessage
            {
                Sender = new MailAddress(Configuration.Account),
                Body = message,
                From = new MailAddress(Configuration.Account),
                IsBodyHtml = isBodyHtml,
                Subject = subject,
                To = { email },
            };

            if (emailAttachment?.Any() == true)
            {
                foreach (var attachment in emailAttachment)
                {
                    // This stream is automatically disposed when the MailMessage is disposed
                    var memStream = new MemoryStream(attachment.Content) { Position = 0 };
                    
                    var contentType = MimeTypes.GetMimeType(attachment.Name);
                    var reportAttachment = new Attachment(memStream, contentType);
                    reportAttachment.ContentDisposition.FileName = attachment.Name;
                    
                    result.Attachments.Add(reportAttachment);
                }
            }
            
            if (!string.IsNullOrWhiteSpace(Configuration.Bcc))
            {
                var addresses = Configuration.Bcc.Split(new[] { ";", "," }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var address in addresses)
                {
                    result.Bcc.Add(address);
                }
            }

            return result;
        }

        private SmtpClient CreateClient()
        {
            var client = new SmtpClient
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(Configuration.Account, Configuration.Password),
                Port = Configuration.Port,
                Host = Configuration.Host,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = Configuration.EnableSsl
            };

            return client;
        }
    }
}
