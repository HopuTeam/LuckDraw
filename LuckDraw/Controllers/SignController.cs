using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using LuckDraw.Models;
using LuckDraw.Handles;

namespace LuckDraw.Controllers
{
    [AllowAnonymous]
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

        [HttpGet]
        public IActionResult Forget()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Forget(Sign sign)
        {
            return View();
        }

        [HttpPost]
        public IActionResult SentMail()
        {
            return View();
        }
    }
}