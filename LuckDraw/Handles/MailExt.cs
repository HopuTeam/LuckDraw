using System.Net;
using System.Net.Mail;
using System.Text;

namespace LuckDraw.Handles
{
    public static class MailExt
    {
        /// <summary>
        /// 邮件发送
        /// </summary>
        public static bool SendMail(string mail, string title, string content)
        {
            try
            {
                SmtpClient client = new SmtpClient
                {
                    Host = "smtp.ym.163.com",//设置SMTP地址
                    UseDefaultCredentials = true,
                    EnableSsl = true,
                    Port = 994,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("work@echocode.club", "9qC3uNQJRy")
                };
                MailMessage Message = new MailMessage
                {
                    From = new MailAddress("work@echocode.club")//一般与发件人保持一致
                };
                Message.To.Add(mail);//要发送的地址
                Message.Subject = title;//标题
                Message.Body = content;//内容
                Message.SubjectEncoding = Encoding.UTF8;
                Message.BodyEncoding = Encoding.UTF8;
                Message.Priority = MailPriority.High;
                Message.IsBodyHtml = true;//允许HTML格式
                client.Send(Message);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}