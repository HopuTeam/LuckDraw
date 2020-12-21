using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class UserController : Controller
    {
        //string code;
        private CoreEntities EF { get; }
        public UserController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Signs.FirstOrDefault(x => x.ID == HttpContext.Session.GetModel<Sign>("User").ID));
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.SetModel("User", null);
            return Content("success");
        }

        [HttpGet]
        public IActionResult Auth(string Code, Sign sign)
        {
            if (Code == HttpContext.Session.GetString("Code"))
            {
                var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
                if (mod == null)
                {
                    return Content("邮箱或帐号错误");
                }
                else
                {
                    mod.Status = true;

                    if (EF.SaveChanges() > 0)
                        return Content("<script>alert('认证成功');window.location.href='/User/Index';</script>", "text/html", System.Text.Encoding.UTF8);
                    else
                        return Content("认证失败，请重试");
                }
            }
            else
            {
                return Content("认证码错误");
            }
        }

        [HttpPost]
        public IActionResult SendMail()
        {
            var mod = HttpContext.Session.GetModel<Sign>("User");
            if (mod == null)
            {
                return Redirect("/Sign/Index");
            }
            else
            {
                Random random = new Random();
                HttpContext.Session.SetString("Code", Security.MD5Encrypt16(random.Next(0, 9999).ToString()).Substring(random.Next(1, 9), 6).ToUpper());
                if (EF.Signs.FirstOrDefault(x => x.Email == mod.Email) == null)
                {
                    return Content("邮箱错误");
                }
                else
                {
                    if (MailExt.SendMail(mod.Email, "账户验证操作", $"尊敬的用户 { mod.Account }：<br />您正在进行<span style='color:skyblue;'>账户认证</span>操作！<br />请点击[<a href='https://localhost:44318/User/Auth?Code={ HttpContext.Session.GetString("Code") }&Account={ mod.Account }&Email={ mod.Email }'>本链接</a>]进行认证。<br />请注意谨防验证码泄露，保护账号安全！"))
                        return Content("success");
                    else
                        return Content("邮件发送失败");
                }
            }
        }
    }
}