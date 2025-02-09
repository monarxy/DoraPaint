using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MyApp
{
    public class SMTP
    {
        public void SendMessage(string text, string adress)
        {
            string smtpServ = "smtp.gmail.com";
            int smtpPort = 587;
            string smtpUserName = "michael14183@gmail.com";
            string smtpPassword = "ycfn mgny fzkk izih";
            using (SmtpClient smtpClient = new SmtpClient(smtpServ, smtpPort))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential(smtpUserName, smtpPassword);
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(smtpUserName);
                    mail.To.Add(adress);
                    mail.Subject = "Письмо от DoraPaint";
                    mail.Body = $"{text}";
                    smtpClient.Send(mail);
                }
            }
        }
    }
}
