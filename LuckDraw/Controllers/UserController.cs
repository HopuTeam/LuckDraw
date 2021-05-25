using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Text;

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
            if (mod.Email != sign.Email)
            {
                mod.Email = sign.Email;
                mod.Status = false;
            }

            if (sign.Password != null)
            {
                mod.Password = Security.MD5Encrypt32(sign.Password);
            }

            try
            {
                EF.SaveChanges();
                HttpContext.Session.SetModel("User", EF.Signs.FirstOrDefault(x => x.ID == ID));
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult EditUser(int ID)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");
            return View(EF.Signs.FirstOrDefault(x => x.ID == ID));
        }
        [HttpPost]
        public IActionResult EditUser(Sign sign)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");

            try
            {
                var mod = EF.Signs.FirstOrDefault(x => x.ID == sign.ID);
                mod.Account = sign.Account;
                mod.Email = sign.Email;
                mod.Password = Security.MD5Encrypt32(sign.Password);
                mod.Identity = sign.Identity;
                EF.SaveChanges();

                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(Sign sign)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");
            try
            {
                EF.Add(sign);
                EF.SaveChanges();

                return Json(new { code = 0, status = true, message = "添加成功" });
            }
            catch (Exception ex)
            {
                return Json(new { code = 0, status = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.SetModel("User", null);
            return Content("success");
        }

        //后台删除用户
        [HttpPost]
        public IActionResult DelUser(int ID)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");

            if (EF.Signs.FirstOrDefault(x => x.Identity == 1).ID == ID)
                return Content("此账户禁止被修改");

            if (EF.Signs.Where(x => x.Identity == 1).ToList().Count <= 1)
                return Content("系统至少要存在一个管理员");

            if (ID == HttpContext.Session.GetModel<Sign>("User").ID)
                return Content("管理员不允许删除自己");

            //清空用户抽奖项目
            var draws = EF.Draws.Where(x => x.SignID == ID);
            foreach (var draw in draws)
                EF.Remove(draw);
            //清空用户抽奖项以及奖项绑定关系
            var lucks = EF.Lucks.Where(x => x.SignID == ID);
            foreach (var luck in lucks)
            {
                foreach (var luckDraw in EF.LuckDraws.Where(x => x.LuckID == luck.ID))
                    EF.Remove(luckDraw);
                EF.Remove(luck);
            }
            //删除用户
            EF.Remove(EF.Signs.FirstOrDefault(x => x.ID == ID));

            try
            {
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        //前台用户自主注销
        [HttpPost]
        public IActionResult ZhuXiao(int ID)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity == 1)
                return Content("管理员账户无法注销");

            ID = HttpContext.Session.GetModel<Sign>("User").ID;
            var draws = EF.Draws.Where(x => x.SignID == ID);
            //清空抽奖项目
            foreach (var draw in draws)
                EF.Remove(draw);
            //清空用户抽奖项以及奖项绑定关系
            var lucks = EF.Lucks.Where(x => x.SignID == ID);
            foreach (var luck in lucks)
            {
                foreach (var luckDraw in EF.LuckDraws.Where(x => x.LuckID == luck.ID))
                    EF.Remove(luckDraw);
                EF.Remove(luck);
            }
            //删除用户
            EF.Remove(EF.Signs.FirstOrDefault(x => x.ID == ID));
            //清空Session
            HttpContext.Session.SetModel("User", null);

            try
            {
                EF.SaveChanges();
                return Content("success");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult GetAcc()
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity == 1)
                return Content("accept");
            return Content("empty data");
        }

        public IActionResult Manager()
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return View("/Views/Error/Index.cshtml");
            return View();
        }

        [HttpPost]
        public IActionResult MagList(int page = 1, int limit = 10)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return Json(new { code = 0, count = 0, msg = "账户权限异常" });
            var mod = (from s in EF.Signs
                       select new
                       {
                           s.ID,
                           s.Account,
                           s.Email,
                           s.Status,
                           s.Identity
                       }).ToList();
            var info = new
            {
                code = 0,
                msg = "",
                count = mod.Count,
                data = mod.Skip((page - 1) * limit).Take(limit)
            };
            return Json(info);
        }

        [HttpPost]
        public IActionResult SwichStatus(int SignID)
        {
            if (HttpContext.Session.GetModel<Sign>("User").Identity != 1)
                return Content("账户权限异常");

            if (SignID == 1)
                return Content("此账户禁止被修改");

            if (EF.Signs.Where(x => x.Identity == 1).ToList().Count <= 1)
                return Content("系统至少要存在一个管理员");

            if (SignID == HttpContext.Session.GetModel<Sign>("User").ID)
                return Content("无法更改自身的权限");

            var mod = EF.Signs.FirstOrDefault(x => x.ID == SignID);
            if (mod.Status)
                mod.Status = false;
            else
                mod.Status = true;

            try
            {
                EF.SaveChanges();
                return Content("操作成功");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Auth(string Code, Sign sign)
        {
            if (Code != HttpContext.Session.GetString("Code"))
                return Content("<script>alert('认证码错误');window.location.href='/User/Index';</script>", "text/html", Encoding.UTF8);

            var mod = EF.Signs.FirstOrDefault(x => x.Account == sign.Account && x.Email == sign.Email);
            if (mod == null)
                return Content("<script>alert('认证信息异常');window.location.href='/User/Index';</script>", "text/html", Encoding.UTF8);

            mod.Status = true;

            if (EF.SaveChanges() > 0)
                return Content("<script>alert('认证成功');window.location.href='/User/Index';</script>", "text/html", Encoding.UTF8);
            else
                return Content("<script>alert('认证失败，请重试');window.location.href='/User/Index';</script>", "text/html", Encoding.UTF8);
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