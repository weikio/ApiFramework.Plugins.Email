using System;

namespace Weikio.ApiFramework.Plugins.Email
{
    public class ApiFactory
    {
        public Type Create(EmailOptions configuration)
        {
            return typeof(EmailSender);
        }
    }
}
