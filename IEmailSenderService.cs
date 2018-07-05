using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IEmailSenderService
    {
        void Send(
            string senderDisplayName,
            string senderEmailAddress,
            string recipientDisplayName,
            string recipientEmailAddress,
            string subject,
            string textBody,
            string htmlBody
            );
    }
}
