using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyNamespace;

namespace Weikio.ApiFramework.Plugins.Email.Sample
{
    [Route("attachmenttest")]
    public class AttachmentTestController : ControllerBase
    {
        // GET
        public async Task<IActionResult> Index()
        {
            var httpClient = new HttpClient();
            var client = new EmailSenderClient(httpClient);

            var email = new MyNamespace.Email
            {
                Message = "Testing api", EmailAddress = "mikael@adafy.com", Subject = "Testing", Attachments = new List<MyNamespace.EmailAttachment>(),
            };


            var atch1 = new MyNamespace.EmailAttachment()
            {
                Content = await System.IO.File.ReadAllBytesAsync("attachments/bing.pdf"),
                Name = "bing.pdf"
            };
            
            var atch2 = new MyNamespace.EmailAttachment()
            {
                Content = await System.IO.File.ReadAllBytesAsync("attachments/google.png"),
                Name = "google.png"
            };
            
            email.Attachments.Add(atch1);
            email.Attachments.Add(atch2);

            await client.CreateAsync(email);

            return Ok();
        }
    }
}
