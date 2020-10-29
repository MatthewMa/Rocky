using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rocky.Utility
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public MailJetSettings MailJetSettings { get; set; }
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string email, string subject, string body)
        {
            MailJetSettings = _configuration.GetSection("MailJet").Get<MailJetSettings>();
            MailjetClient client = new MailjetClient(MailJetSettings.ApiKey, MailJetSettings.SecretKey)
            {
                Version = ApiVersion.V3_1,
            };
            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource,
            }
             .Property(Send.Messages, new JArray {
                 new JObject {
                  {
                   "From",
                   new JObject {
                    {"Email", "masihua721@gmail.com"},
                    {"Name", "Rocky App Admin"}
                   }
                  }, {
                   "To",
                   new JArray {
                    new JObject {
                     {
                      "Email",
                      email
                     }, {
                      "Name",
                      "DotNetMastery"
                     }
                    }
                   }
                  }, {
                   "Subject",
                   subject
                  }, {
                   "HTMLPart",
                   body
                  }
                 }
             });
            MailjetResponse response = await client.PostAsync(request);
        }
    }
}
