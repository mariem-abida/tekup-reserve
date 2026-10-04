using Microsoft.AspNetCore.Identity;

namespace TEKUP_RESERVE.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        // plus tard : Student / Teacher, etc.
    }
}
