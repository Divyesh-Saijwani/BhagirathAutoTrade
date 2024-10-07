using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using BhagirathFincareUtil;

namespace BhagirathFincareCore.Lib
{
    public partial class Email
    {
        public static bool SendEmail(string to, string subject, string body)
        {
            try
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "mail.bhagirathfincare.in";
                    smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("welcome@bhagirathfincare.in", "Test#123");
                    using (var objMailMessage = new MailMessage("welcome@bhagirathfincare.in", to, subject, body))
                    {
                        objMailMessage.IsBodyHtml = true;
                        smtp.Send(objMailMessage);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogError(ex.Message);
                return false;
                //throw;
            }
        }

        public static bool SendInvoiceEmail(string to, string subject, string body, string name, string invoiceno)
        {
            try
            {
                StringBuilder textstring = new StringBuilder();
                textstring.Append("Hi " + name + " ,");
                textstring.AppendLine("I hope you are well.");
                textstring.AppendLine("I just wanted to drop you a quick note to remind you in respect of our invoice " + invoiceno + " is due for payment.");
                textstring.AppendLine("Kindly find the attachment for your unpaid invoice.");
                textstring.AppendLine("I would be really grateful if you could confirm that everything is on track for payment or send us the payment paid slip.");
                textstring.AppendLine("\r\n");
                textstring.AppendLine("Best regards,");
                textstring.AppendLine("\r\n");
                textstring.AppendLine("Bhagirath Fincare");
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "mail.bhagirathfincare.in";
                    smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("welcome@bhagirathfincare.in", "Test#123");
                    using (var objMailMessage = new MailMessage("welcome@bhagirathfincare.in", to, subject, textstring.ToString()))
                    {
                        objMailMessage.IsBodyHtml = true;
                        objMailMessage.Attachments.Add(new Attachment(body));
                        smtp.Send(objMailMessage);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogError(ex.Message);
                return false;
                //throw;
            }
        }

        public static bool SendPaidInvoiceEmail(string to, string subject, string body, string name, string invoiceno, double total)
        {
            try
            {
                StringBuilder textstring = new StringBuilder();
                textstring.Append("Hello " + name + " ," + "<br/><br/>");
                textstring.AppendLine("");
                textstring.Append("We earnestly acknowledge your payment of " + total + ", which we received from you for the recompense of your withstanding amount for the last month’s deal with our company. (Describe in your own words).<br/><br/>");
                textstring.AppendLine("");
                textstring.AppendLine("With the payment of " + total + " , we would like to inform that you have paid all your debts and there is no balance amount remaining for payment. We sincerely appreciate your promptness regarding all payments from your side. (Cordially describe all about the situation). You have always fulfilled the promises made by you regarding deadlines and payments. We admire your sincerity and dedication that you have always maintained as a customer. (Explain your expectation).<br/><br/>");
                textstring.AppendLine("");
                textstring.AppendLine("We would like to take this opportunity to thank you for being a valued customer with us for so long. (Cordially describe your greetings and requirements). We look forward to continuing being in business with you in the long run.<br/><br/>");
                textstring.AppendLine("");
                textstring.AppendLine("Best regards,<br/>");
                textstring.AppendLine("");
                textstring.AppendLine("Bhagirath Fincare");
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = "mail.bhagirathfincare.in";
                    smtp.Port = 587;
                    smtp.EnableSsl = false;
                    smtp.UseDefaultCredentials = false;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Credentials = new NetworkCredential("welcome@bhagirathfincare.in", "Test#123");
                    using (var objMailMessage = new MailMessage("welcome@bhagirathfincare.in", to, subject, textstring.ToString()))
                    {
                        objMailMessage.IsBodyHtml = true;
                        objMailMessage.Body = textstring.ToString();
                        objMailMessage.Attachments.Add(new Attachment(body));
                        smtp.Send(objMailMessage);
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                ex.LogError(ex.Message);
                return false;
                //throw;
            }
        }
    }
}
