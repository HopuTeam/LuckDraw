using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckDraw.Controllers
{
    public class DrawController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult add()
        {
         return  Redirect("Draw/index");
        }




        // 启用codefirst迁移    enable-migrations
        // 添加一个迁移文件      add-migration [name]     -- add-migration one
        // 更新数据库           update-database








    }
}
