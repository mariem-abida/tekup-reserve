using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TEKUP_RESERVE.Data;
using TEKUP_RESERVE.Models;

namespace TEKUP_RESERVE.Controllers
{
    [Authorize]  // il faut être connecté pour réserver / voir les réservations
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var query = _context.Bookings
                .Include(b => b.Room)
                .AsQueryable();

            // Si ce n'est PAS un Admin → on filtre par UserId
            if (!User.IsInRole("Admin"))
            {
                var userName = User.Identity?.Name;

                if (!string.IsNullOrEmpty(userName))
                {
                    // On a stocké userName dans Booking.UserId
                    query = query.Where(b => b.UserId == userName);
                }
                else
                {
                    // utilisateur non identifié = aucune réservation
                    query = query.Where(b => false);
                }
            }

            var bookings = await query
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            return View(bookings);
        }




        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            // On enlève UserId et Room de la validation, ils ne viennent pas du formulaire
            ModelState.Remove("UserId");
            ModelState.Remove("Room");

            if (booking.EndTime <= booking.StartTime)
            {
                ModelState.AddModelError("", "L'heure de fin doit être supérieure à l'heure de début.");
            }

            // Vérification de conflit : même salle + créneau qui se chevauche
            bool conflict = await _context.Bookings.AnyAsync(b =>
                b.RoomId == booking.RoomId &&
                (
                    (booking.StartTime >= b.StartTime && booking.StartTime < b.EndTime) ||
                    (booking.EndTime > b.StartTime && booking.EndTime <= b.EndTime) ||
                    (booking.StartTime <= b.StartTime && booking.EndTime >= b.EndTime)
                )
            );

            if (conflict)
            {
                ModelState.AddModelError("", "Conflit : cette salle est déjà réservée sur ce créneau.");
            }

            if (!ModelState.IsValid)
            {
                ViewData["RoomId"] = new SelectList(_context.Rooms, "Id", "Name", booking.RoomId);
                return View(booking);
            }

            // Ici on est sûr que le modèle est valide
            var userName = User.Identity?.Name;
            booking.UserId = string.IsNullOrEmpty(userName) ? "Anonyme" : userName;

            _context.Add(booking);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Réservation enregistrée avec succès ✔";

            return RedirectToAction(nameof(Index));
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Room)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
