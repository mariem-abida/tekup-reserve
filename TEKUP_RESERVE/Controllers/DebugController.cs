using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TEKUP_RESERVE.Models;

namespace TEKUP_RESERVE.Controllers
{
    public class DebugController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DebugController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager,
                               SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        [Authorize]
        public async Task<IActionResult> FixAdmin()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Content("Aucun utilisateur connecté.");

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");

            // 🔹 Rafraîchir le cookie d’authentification
            await _signInManager.RefreshSignInAsync(user);

            return Content($"L'utilisateur {user.Email} a maintenant le rôle ADMIN ✔");
        }
    }

}
