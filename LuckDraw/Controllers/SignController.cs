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
            Sign mod;
            sign.Password = Security.MD5Encrypt32(sign.Password);
            if (sign.Account.Contains("@"))
            {
                mod = (from s in EF.Signs
                       where s.Email == sign.Account && s.Password == sign.Password
                       select s).FirstOrDefault();
            }
            else
            {
                mod = (from s in EF.Signs
                       where s.Account == sign.Account && s.Password == sign.Password
                       select s).FirstOrDefault();
            }

            if (mod == null)
                return Content("用户名或密码错误");

            HttpContext.Session.SetModel("User", mod);
            return Content("success");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Sign sign)
        {
            if (EF.Signs.FirstOrDefault(x => x.Account == sign.Account) != null)
                return Content("用户名被占用");

            if (EF.Signs.FirstOrDefault(x => x.Email == sign.Email) != null)
                return Content("邮箱已被绑定");

            sign.Status = false;
            sign.Identity = 0;
            sign.Password = Security.MD5Encrypt32(sign.Password);
            EF.Signs.Add(sign);

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("表单数据异常");
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
            if (Code != code)
                return Content("验证码错误");

            var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
            if (mod == null)
                return Content("邮箱或帐号错误");

            mod.Password = Security.MD5Encrypt32(sign.Password);

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("重置密码失败，请重试");
        }

        [HttpPost]
        public IActionResult SendMail(string Account, string mail)
        {
            Random random = new Random();
            code = Security.MD5Encrypt16(random.Next(0, 9999).ToString()).Substring(random.Next(1, 9), 6).ToUpper();
            if (EF.Signs.FirstOrDefault(x => x.Account == Account && x.Email == mail) == null)
                return Content("邮箱或帐号错误");

            if (MailExt.SendMail(mail, "找回密码操作", $"尊敬的用户 { Account }：<br />您正在进行<span style='color:red;'>找回密码</span>操作！<br />本次操作的验证码是：<span style='color:red;'>{ code }</span>。<br />请注意谨防验证码泄露，保护账号安全！"))
                return Content("success");
            else
                return Content("邮件发送失败");
        }
    }
}