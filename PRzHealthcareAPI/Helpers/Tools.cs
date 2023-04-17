using PRzHealthcareAPI.Models;
using System.Net.Mail;

namespace PRzHealthcareAPI.Helpers
{
    public static class Tools
    {
        private static string _senderEmail = "noreply@arcussoft.com.pl";
        private static string _passwordEmail = "Guf26409";
        private static string _host = "smtp.office365.com";
        private static int _port = 587;
        private static int _timeout = 10000;
        private static string _starterLink = $@"http://localhost:3000";
        public static bool SendRegistrationMail(Account user, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = _port,
                    Host = _host,
                    Timeout = _timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(_senderEmail, _passwordEmail),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(_senderEmail);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                //objeto_mail.Priority = MailPriority.High;

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template.Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}").Replace("@@LINK", $@"{_starterLink}/account/confirm-mail?hashCode={user.Acc_RegistrationHash}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool SendPasswordReminder(Account user, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = _port,
                    Host = _host,
                    Timeout = _timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(_senderEmail, _passwordEmail),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(_senderEmail);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template.Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}").Replace("@@LINK", $@"{_starterLink}/account/reset-passwordcheck?hashCode={user.Acc_ReminderHash}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static bool SendVisitConfirmation(Account user, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = _port,
                    Host = _host,
                    Timeout = _timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(_senderEmail, _passwordEmail),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(_senderEmail);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template.Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}");

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
