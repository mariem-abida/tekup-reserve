using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TEKUP_RESERVE.Data;
using TEKUP_RESERVE.Models;

namespace TEKUP_RESERVE.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            AppDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------- 1) DASHBOARD ----------
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;

            var totalRooms = await _context.Rooms.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var bookingsToday = await _context.Bookings
                .CountAsync(b => b.StartTime.Date == today);

            var upcoming = await _context.Bookings
                .Include(b => b.Room)
                .OrderBy(b => b.StartTime)
                .Where(b => b.StartTime >= today)
                .Take(5)
                .ToListAsync();

            var topRoomsData = await _context.Bookings
                .Include(b => b.Room)
                .GroupBy(b => b.Room)
                .OrderByDescending(g => g.Count())
                .Take(5)
                .Select(g => new AdminDashboardViewModel.TopRoomItem
                {
                    RoomName = g.Key.Name,
                    ReservationsCount = g.Count()
                })
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalRooms = totalRooms,
                TotalBookings = totalBookings,
                BookingsToday = bookingsToday,
                UpcomingBookings = upcoming,
                TopRooms = topRoomsData
            };

            return View(model);
        }

        // ---------- 2) LISTE DES UTILISATEURS ----------
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserWithRolesViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserWithRolesViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        // ---------- 3) EDITER LES RÔLES D'UN UTILISATEUR ----------
        [HttpGet]
        public async Task<IActionResult> EditUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserRolesViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                FullName = user.FullName
            };

            foreach (var role in allRoles)
            {
                model.Roles.Add(new RoleSelectionViewModel
                {
                    RoleName = role.Name!,
                    Selected = userRoles.Contains(role.Name!)
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserRoles(EditUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null) return NotFound();

            var currentRoles = await _userManager.GetRolesAsync(user);
            var selectedRoles = model.Roles
                .Where(r => r.Selected)
                .Select(r => r.RoleName)
                .ToList();

            // On enlève tous les rôles actuels
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded)
            {
                ModelState.AddModelError("", "Erreur lors de la suppression des anciens rôles.");
                return View(model);
            }

            // On ajoute les rôles sélectionnés (Admin / Teacher / Student)
            var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
            if (!addResult.Succeeded)
            {
                ModelState.AddModelError("", "Erreur lors de l'ajout des nouveaux rôles.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Rôles de l'utilisateur mis à jour avec succès ✔";

            return RedirectToAction(nameof(Users));
        }
    }
}
