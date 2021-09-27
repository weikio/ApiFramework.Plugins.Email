using System;
using System.Collections.Generic;
using System.Text;

namespace Weikio.ApiFramework.Plugins.Email
{
    public class EmailSendingResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
