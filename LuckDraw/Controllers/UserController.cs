using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class UserController : BaseController
    {
        private CoreEntities EF { get; }
        public UserController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View(EF.Signs.FirstOrDefault(x => x.ID == HttpContext.Session.GetModel<Sign>("User").ID));
        }

        [HttpPost]
        public IActionResult Edit(Sign sign)
        {
            var ID = HttpContext.Session.GetModel<Sign>("User").ID;
            var mod = EF.Signs.FirstOrDefault(x => x.ID == ID);
            if (sign.Password == null)
            {
                if (mod.Email == sign.Email)
                    return Content("未进行任何修改");

                mod.Email = sign.Email;
                mod.Status = false;
            }
            else
            {
                if (mod.Email != sign.Email)
                {
                    mod.Email = sign.Email;
                    mod.Status = false;
                }
                mod.Password = Security.MD5Encrypt32(sign.Password);
            }
            if (EF.SaveChanges() <= 0)
                return Content("数据更新异常");

            HttpContext.Session.SetModel("User", EF.Signs.FirstOrDefault(x => x.ID == ID));
            return Content("success");
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
            if (Code != HttpContext.Session.GetString("Code"))
                return Content("<script>alert('认证码错误');window.location.href='/User/Index';</script>", "text/html", System.Text.Encoding.UTF8);

            var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
            if (mod == null)
                return Content("<script>alert('认证信息异常');window.location.href='/User/Index';</script>", "text/html", System.Text.Encoding.UTF8);

            mod.Status = true;

            if (EF.SaveChanges() > 0)
                return Content("<script>alert('认证成功');window.location.href='/User/Index';</script>", "text/html", System.Text.Encoding.UTF8);
            else
                return Content("<script>alert('认证失败，请重试');window.location.href='/User/Index';</script>", "text/html", System.Text.Encoding.UTF8);
        }

        [HttpPost]
        public IActionResult SendMail()
        {
            var mod = HttpContext.Session.GetModel<Sign>("User");
            Random random = new Random();
            HttpContext.Session.SetString("Code", Security.MD5Encrypt32(random.Next(0, 9999).ToString()).Substring(random.Next(1, 16), 6).ToUpper());

            if (mod.Email == null)
                return Content("邮箱信息错误");

            if (MailExt.SendMail(mod.Account, mod.Email, "账户验证操作", $"尊敬的用户{ mod.Account }：<br />您正在进行<span style='color:skyblue;'>账户认证</span>操作！<br />请点击[<a href='https://luck.lzzy.ml/User/Auth?Code={ HttpContext.Session.GetString("Code") }&Account={ mod.Account }&Email={ mod.Email }&AC={ Security.MD5Encrypt32(DateTime.Now.ToString()).Substring(random.Next(1, 12), 9) }'>本链接</a>]进行认证。<br />请注意谨防验证码泄露，保护账号安全！"))
                return Content("success");
            else
                return Content("邮件发送失败");
        }
    }
}