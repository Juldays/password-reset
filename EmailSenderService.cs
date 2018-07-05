using Newtonsoft.Json.Linq;
using Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class EmailSenderService : IEmailSenderService
    {
        readonly IConfiguration confg;

        public EmailSenderService(IConfiguration confg)
        {
            this.confg = confg;
        }
        public void Send(
            string senderDisplayName, 
            string senderEmailAddress, 
            string recipientDisplayName, 
            string recipientEmailAddress, 
            string subject, 
            string textBody, 
            string htmlBody
            )
        {
            var apiKey = confg.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(senderEmailAddress, senderDisplayName);
            var to = new EmailAddress(recipientEmailAddress, recipientDisplayName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, textBody, htmlBody);
            var response = client.SendEmailAsync(msg).Result;
            var statusCode = (int)response.StatusCode;

            if (statusCode < 200 || statusCode >= 300)
            {
                string exceptionMsg;
                var body = response.Body.ReadAsStringAsync().Result;

                try
                {
                    JObject bodyData = JObject.Parse(body);
                    exceptionMsg = bodyData["errors"][0]["message"].ToString();
                }
                catch (Exception)
                {
                    exceptionMsg = body;
                }
                
                throw new Exception($"Exception: {response.StatusCode}, {exceptionMsg}");
            }
        }
    }
}
