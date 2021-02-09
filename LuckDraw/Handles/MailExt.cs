using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;

namespace LuckDraw.Handles
{
    public static class MailExt
    {
        /// <summary>
        /// 邮件发送
        /// </summary>
        /// <param name="name">收件人昵称</param>
        /// <param name="mail">收件人邮件地址</param>
        /// <param name="title">邮件标题</param>
        /// <param name="content">邮件内容</param>
        /// <returns>true or false</returns>
        public static bool SendMail(string name, string mail, string title, string content)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Work Job", "work@echocode.club"));//发件人名、发件人邮箱
                message.To.Add(new MailboxAddress(name, mail));//收件人昵称、收件人地址
                message.Subject = title;//标题
                message.Body = new TextPart(TextFormat.Html) { Text = content };//内容(HTML格式)
                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.ym.163.com", 994, SecureSocketOptions.SslOnConnect);//SMTP地址、端口、加密方式
                    client.Authenticate("work@echocode.club", "9qC3uNQJRy");
                    client.Send(message);
                    client.Disconnect(true);
                }

                return true;
            }
            catch// (Exception ex)
            {
                return false;
            }
        }
    }
}