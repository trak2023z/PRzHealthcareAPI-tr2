using PRzHealthcareAPI.Models;
using System.Net.Mail;

namespace PRzHealthcareAPI.Helpers
{
    public static class Tools
    {
        public static bool SendRegistrationMail(Account user, NotificationType notification)
        {
            try
            {
                string senderEmail = "przhealthcare@vp.pl";
                string passwordEmail = "Politechnika123!";

                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = 465,
                    Host = "smtp.poczta.onet.pl",
                    Timeout = 10000,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(senderEmail, passwordEmail),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(senderEmail);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Priority = MailPriority.High;

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template.Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}").Replace("@@LINK", $@"http://192.168.56.1:5000/account/confirm-mail/{user.Acc_RegistrationHash}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
