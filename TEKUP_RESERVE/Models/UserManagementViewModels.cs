using System.Collections.Generic;

namespace TEKUP_RESERVE.Models
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class EditUserRolesViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? FullName { get; set; }

        public List<RoleSelectionViewModel> Roles { get; set; } = new();
    }

    public class RoleSelectionViewModel
    {
        public string RoleName { get; set; } = string.Empty;
        public bool Selected { get; set; }
    }
}
