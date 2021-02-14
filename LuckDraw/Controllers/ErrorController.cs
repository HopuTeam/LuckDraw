using Microsoft.AspNetCore.Mvc;

namespace LuckDraw.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/Error/{0}")]
        public IActionResult Page()
        {
            //跳转错误页
            if (Response.StatusCode == 404)
                return View("/Views/Error/Index.cshtml");
            return View();
        }
    }
}