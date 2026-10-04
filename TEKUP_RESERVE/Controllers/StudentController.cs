// Controllers/StudentController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TEKUP_RESERVE.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        public IActionResult Calendar()
        {
            return View();   // => cherche Views/Student/Calendar.cshtml
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }
    }
}
