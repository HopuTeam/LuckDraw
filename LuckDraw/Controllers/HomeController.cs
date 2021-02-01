using LuckDraw.Handles;
using LuckDraw.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace LuckDraw.Controllers
{
    public class HomeController : BaseController
    {
        private CoreEntities EF { get; }
        public HomeController(CoreEntities _ef) : base(_ef)
        {
            EF = _ef;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GetInfo()
        {
            var mod = (from sign in EF.Signs
                       where sign.ID == HttpContext.Session.GetModel<Sign>("User").ID
                       select new
                       {
                           username = sign.Account,
                           luck = (from luck in EF.Lucks
                                   where luck.SignID == sign.ID && luck.ParentID > 0
                                   select luck).Count(),
                           draw = (from draw in EF.Draws
                                   where draw.SignID == sign.ID
                                   select draw).Count()
                       }).FirstOrDefault();

            var model = new
            {
                //登录用户名字
                userName = mod.username,
                //奖项的数量
                drawCount = mod.draw,
                //项目的数量
                luckCount = mod.luck
            };
            return Json(model);
        }
    }
}