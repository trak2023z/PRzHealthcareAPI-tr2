using PRzHealthcareAPI.Models;
using System.Net.Mail;
using System.Security.Cryptography;

namespace PRzHealthcareAPI.Helpers
{
    public static class Tools
    {
        private static string _senderEmail = "noreply@arcussoft.com.pl";
        private static string _passwordEmail = "Guf26409";
        private static string _host = "smtp.office365.com";
        private static int _port = 587;
        private static int _timeout = 10000;
        private static string _starterLink = $@"https://przhealthcare-app.azurewebsites.net";

        /// <summary>
        /// Wysłanie powiadomienia o rejestracji użytkownika
        /// </summary>
        /// <param name="emailSettings">Obiekt konfiguracji skrzynki pocztowej</param>
        /// <param name="user">Obiekt użytkownika docelowego</param>
        /// <param name="notification">Obiekt notyfikacji</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public static bool SendRegistrationMail(EmailSettings emailSettings, Account user, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host,
                    Timeout = emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(emailSettings.Address, emailSettings.Password),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(emailSettings.Address);
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

        /// <summary>
        /// Wysłanie powiadomienia o przypomnieniu hasła użytkownika
        /// </summary>
        /// <param name="emailSettings">Obiekt konfiguracji skrzynki pocztowej</param>
        /// <param name="user">Obiekt użytkownika docelowego</param>
        /// <param name="notification">Obiekt notyfikacji</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public static bool SendPasswordReminder(EmailSettings emailSettings, Account user, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host,
                    Timeout = emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(emailSettings.Address, emailSettings.Password),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(emailSettings.Address);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template
                    .Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}")
                    .Replace("@@LINK", $@"{_starterLink}/account/password-restart?hashCode={user.Acc_ReminderHash}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Wysłanie powiadomienia o potwierdzeniu wizyty użytkownika
        /// </summary>
        /// <param name="emailSettings">Obiekt konfiguracji skrzynki pocztowej</param>
        /// <param name="user">Obiekt użytkownika docelowego</param>
        /// <param name="newEvent">Obiekt terminu wizyty</param>
        /// <param name="notification">Obiekt notyfikacji</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public static bool SendVisitConfirmation(EmailSettings emailSettings, Account user, Event newEvent, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host,
                    Timeout = emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(emailSettings.Address, emailSettings.Password),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(emailSettings.Address);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template
                    .Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}")
                    .Replace("@@TERMIN", $@"{newEvent.Eve_TimeFrom}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Wysłanie powiadomienia o anulowaniu wizyty użytkownika
        /// </summary>
        /// <param name="emailSettings">Obiekt konfiguracji skrzynki pocztowej</param>
        /// <param name="user">Obiekt użytkownika docelowego</param>
        /// <param name="cancelledEvent">Obiekt anulowanego terminu wizyty</param>
        /// <param name="notification">Obiekt notyfikacji</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public static bool SendVisitCancellation(EmailSettings emailSettings, Account user, Event cancelledEvent, NotificationType notification)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host,
                    Timeout = emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(emailSettings.Address, emailSettings.Password),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(emailSettings.Address);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template
                    .Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}")
                    .Replace("@@TERMIN", $@"{cancelledEvent.Eve_TimeFrom.ToLongDateString()}");

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Wysłanie powiadomienia o zakończeniu wizyty użytkownika wraz z zaświadczeniem COVID
        /// </summary>
        /// <param name="emailSettings">Obiekt konfiguracji skrzynki pocztowej</param>
        /// <param name="user">Obiekt użytkownika docelowego</param>
        /// <param name="finishedEvent">Obiekt zakończonego terminu wizyty</param>
        /// <param name="notification">Obiekt notyfikacji</param>
        /// <returns>Poprawność wykonania funkcji</returns>
        public static bool SendVisitCertificate(EmailSettings emailSettings, Account user, Event finishedEvent, NotificationType notification, MemoryStream attachmentFile)
        {
            try
            {
                MailMessage objeto_mail = new MailMessage();
                SmtpClient client = new SmtpClient
                {
                    Port = emailSettings.Port,
                    Host = emailSettings.Host,
                    Timeout = emailSettings.Timeout,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new System.Net.NetworkCredential(emailSettings.Address, emailSettings.Password),
                    EnableSsl = true
                };
                objeto_mail.From = new MailAddress(emailSettings.Address);
                objeto_mail.To.Add(new MailAddress(user.Acc_Email));

                objeto_mail.Subject = notification.Nty_Name;
                objeto_mail.IsBodyHtml = true;
                objeto_mail.Body = notification.Nty_Template
                    .Replace("@@NAZWA", $@"{user.Acc_Firstname} {user.Acc_Lastname}");

                Attachment attachment = new Attachment(Path.Combine(Path.GetTempPath(), $@"certificate{finishedEvent.Eve_Id}.pdf"));
                objeto_mail.Attachments.Add(attachment);

                client.Send(objeto_mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Generowanie ciągu znaków
        /// </summary>
        /// <param name="bytes">Ilość bajtów do wygenerowania tekstu</param>
        /// <returns></returns>
        public static string CreateRandomToken(int bytes)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(bytes));
        }

        /// <summary>
        /// Konwersja pliku do BASE64
        /// </summary>
        /// <param name="filePath">Ścieżka do pliku</param>
        /// <returns></returns>
        public static string ToBase64Converter(string filePath)
        {
            string daneBinarne;
            if (System.IO.File.Exists(filePath))
            {
                byte[] bufor = System.IO.File.ReadAllBytes(filePath);
                daneBinarne = System.Convert.ToBase64String(bufor);
            }
            else
            {
                return "";
            }
            return daneBinarne;
        }

        /// <summary>
        /// Konwersja BASE64 do pliku
        /// </summary>
        /// <param name="baseCode">kod BASE64</param>
        /// <returns></returns>
        public static string FromBase64Converter(IWebHostEnvironment hostingEnvironment, string baseCode)
        {
            try
            {
                string path = Path.Combine(hostingEnvironment.ContentRootPath, "wydruk.rpt");
                if (File.Exists(path))
                {
                    using (FileStream stream = new FileStream(Path.Combine(hostingEnvironment.ContentRootPath, "wydruk.rpt"), FileMode.Open))
                    {
                        File.Delete(path);
                        stream.Dispose();
                    }
                }


                byte[] tempBytes = Convert.FromBase64String(baseCode);
                File.WriteAllBytes(path, tempBytes);

                return path;
            }
            catch
            {
                return "";
            }
        }

    }
}
