using System.Collections.Generic;

namespace Weikio.ApiFramework.Plugins.Email
{
    public class EmailOptions
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; } = true;
        public string Bcc { get; set; }
        public string DefaultHeaderText { get; set; } = "";
        public string DefaultHeaderHtml { get; set; } = "";
        public string DefaultFooterText { get; set; } = "";
        public string DefaultFooterHtml { get; set; } = "";
        public HashSet<string> AllowedReceivers { get; set; } = new HashSet<string>();
    }
}
