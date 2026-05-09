using Microsoft.AspNetCore.Mvc;

namespace QuanLyDuAn.Controllers
{
    public class ChatDuAnController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
