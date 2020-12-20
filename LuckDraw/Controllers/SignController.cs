using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LuckDraw.Models;
using LuckDraw.Handles;
using Microsoft.AspNetCore.Authorization;

namespace LuckDraw.Controllers
{
    public class SignController : BaseController
    {
        private CoreEntities EF { get; }
        public SignController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Sign(Sign sign)
        {
            sign.Password = Security.MD5Encrypt32(sign.Password);
            var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Password == sign.Password);
            if (mod != null)
            {
                HttpContext.Session.SetModel("User", mod);
                return Content("success");
            }
            else
            {
                return Content("用户名或密码错误");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Sign sign)
        {
            sign.Status = false;
            sign.Identity = 0;
            sign.Password = Security.MD5Encrypt32(sign.Password);
            EF.Signs.Add(sign);
            if (EF.SaveChanges() > 0)
            {
                return Content("success");
            }
            else
            {
                return Content("表单数据异常");
            }
        }

        string code;
        [HttpGet]
        public IActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forget(string Code, Sign sign)
        {
            if (Code == code)
            {
                var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
                if (mod == null)
                {
                    return Content("邮箱或帐号错误");
                }
                else
                {
                    mod.Password = Security.MD5Encrypt32(sign.Password);

                    if (EF.SaveChanges() > 0)
                        return Content("success");
                    else
                        return Content("重置密码失败，请重试");
                }
            }
            else
            {
                return Content("验证码错误");
            }
        }

        [HttpPost]
        public IActionResult SentMail(string Account, string mail)
        {
            Random random = new Random();
            code = random.Next(1000, 9999).ToString();
            if (EF.Signs.FirstOrDefault(x => x.Account == Account && x.Email == mail) == null)
            {
                return Content("邮箱或帐号错误");
            }
            else
            {
                if (MailExt.SendMail(mail, "找回密码操作", $"您本次操作的验证码是{ code }，请注意谨防验证码泄露，保护账号安全！"))
                    return Content("success");
                else
                    return Content("邮件发送失败");
            }
        }
    }
}