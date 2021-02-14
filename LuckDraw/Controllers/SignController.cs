using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class SignController : Controller
    {
        private CoreEntities EF { get; }
        public SignController(CoreEntities _ef)
        {
            EF = _ef;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var user = context.HttpContext.Session.GetModel<Sign>("User");
            if (user != null && user.Identity != 1)
                context.Result = new RedirectResult("/Home/Index");
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
            Sign mod = (from s in EF.Signs
                        where s.Email == sign.Account && s.Password == sign.Password
                        select s).FirstOrDefault();

            if (mod == null)
                mod = (from s in EF.Signs
                       where s.Account == sign.Account && s.Password == sign.Password
                       select s).FirstOrDefault();
            else if (mod == null)
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
        public IActionResult Register(string Code, Sign sign)
        {
            if (EF.Signs.FirstOrDefault(x => x.Account == sign.Account) != null)
                return Content("用户名被占用");

            if (EF.Signs.FirstOrDefault(x => x.Email == sign.Email) != null)
                return Content("邮箱已被绑定");

            if (Code != HttpContext.Session.GetString("Code"))
                return Content("验证码错误");

            sign.Status = false;
            sign.Identity = 0;
            sign.Password = Security.MD5Encrypt32(sign.Password);
            EF.Signs.Add(sign);

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("表单数据异常");
        }

        [HttpGet]
        public IActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forget(string Code, Sign sign)
        {
            if (Code != HttpContext.Session.GetString("Code"))
                return Content("验证码错误");

            var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
            if (mod == null)
                return Content("邮箱或帐号错误");
            else if (mod.Status == false)
                return Content("邮箱验证未通过，请联系管理员");

            mod.Password = Security.MD5Encrypt32(sign.Password);

            if (EF.SaveChanges() > 0)
                return Content("success");
            else
                return Content("重置密码失败，请重试");
        }

        [HttpPost]
        public IActionResult SendMail(string Account, string mail, int type)
        {
            Random random = new Random();
            HttpContext.Session.SetString("Code", Security.MD5Encrypt32(random.Next(0, 9999).ToString()).Substring(random.Next(1, 16), 6).ToUpper());

            string name = string.Empty;
            switch (type)
            {
                case 0:
                    var mod = EF.Signs.FirstOrDefault(x => x.Account == Account && x.Email == mail);
                    if (mod == null)
                        return Content("邮箱或帐号错误");
                    else if (mod.Status == false)
                        return Content("邮箱验证未通过，请联系管理员");
                    name = "找回密码";
                    break;
                case 1:
                    name = "注册账号";
                    break;
                default:
                    return Content("数据非法");
            }

            if (MailExt.SendMail(Account, mail, $"{ name }操作", $"尊敬的用户{ Account }：<br />您正在进行<span style='color:red;'>{ name }</span>操作！<br />本次操作的验证码是：<span style='color:red;'>{ HttpContext.Session.GetString("Code") }</span>。<br />请注意谨防验证码泄露，保护账号安全！"))
                return Content("success");
            else
                return Content("邮件发送失败");
        }
    }
}