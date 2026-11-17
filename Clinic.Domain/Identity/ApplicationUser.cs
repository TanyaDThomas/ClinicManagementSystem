

using Microsoft.AspNetCore.Identity;

namespace Clinic.Domain.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }

    }
}
