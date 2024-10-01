using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace Church.Models
{
    public class SendMail
    {
        public void SendMailToMember(string ToEmailID, string mailBody, string mailtitle)
        {
            try
            {
                var SMTPServerName = "probus.icewarpcloud.in";
                var PortNo = 587;
                var FromEmailId = "qa@probus.co.in";
                var SMTPPassword = "Ui@123@.#";
                var SSL = 1;

                SmtpClient smtp = new SmtpClient(SMTPServerName, PortNo);
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = true;
                smtp.Timeout = 100000;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                smtp.Credentials = new System.Net.NetworkCredential(FromEmailId, SMTPPassword);

                MailMessage mail = new MailMessage(FromEmailId, ToEmailID);
                mail.Subject = mailtitle;
                var GetBody = mailBody;
                mail.Body = GetBody;
                //  mail.Attachments.Add(new Attachment(Fbank.ExportToStream(ExportFormatType.PortableDocFormat), "PaymentSlip.pdf"));
                mail.IsBodyHtml = true;
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                ex.ToString();                
               
            }           
        }
    }
}