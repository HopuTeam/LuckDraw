using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class UserController : Controller
    {
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
        public IActionResult Auth()
        {
            return View(EF.Signs.FirstOrDefault(x => x.ID == HttpContext.Session.GetModel<Sign>("User").ID));
        }
        string code;
        [HttpPost]
        public IActionResult Auth(Sign sign)
        {
            return Content("success");
        }

        [HttpPost]
        public IActionResult SentMail(string mail)
        {
            Random random = new Random();
            code = random.Next(1000, 9999).ToString();
            if (EF.Signs.FirstOrDefault(x => x.Email == mail) == null)
            {
                return Content("邮箱错误");
            }
            else
            {
                if (MailExt.SendMail(mail, "账户验证操作", $"您本次操作的验证码是 <span style='color:red;'>{ code }</span> ，请注意谨防验证码泄露，保护账号安全！"))
                    return Content("success");
                else
                    return Content("邮件发送失败");
            }
        }
    }
}