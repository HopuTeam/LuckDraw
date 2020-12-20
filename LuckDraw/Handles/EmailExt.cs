using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace LuckDraw.Handles
{
    public static class EmailExt
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
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new NetworkCredential("gm@echocode.club", "luoxinaini1314")
                };
                MailMessage Message = new MailMessage
                {
                    From = new MailAddress("gm@echocode.club")//与发件人保持一致(已知163要求)
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