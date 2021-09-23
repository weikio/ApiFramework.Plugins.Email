using System.Collections.Generic;

namespace Weikio.ApiFramework.Plugins.Email
{
    public class Email
    {
        public string EmailAddress { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string HtmlMessage { get; set; }
        public List<EmailAttachment> Attachments { get; set; }
    }

    public class EmailAttachment
    {
        public string Name { get; set; }
        public byte[] Content { get; set; }
    }
}
