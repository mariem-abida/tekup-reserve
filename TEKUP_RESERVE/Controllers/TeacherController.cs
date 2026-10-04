using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TEKUP_RESERVE.Controllers
{
    [Authorize(Roles = "Teacher")] // tu peux mettre juste [Authorize] si tu veux
    public class TeacherController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult Calendar()
        {
            return View();
        }

        public IActionResult Reservations()
        {
            return View();
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
