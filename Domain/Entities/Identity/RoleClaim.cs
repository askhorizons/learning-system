using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class RoleClaim : IdentityRoleClaim<string>
    {
        public virtual AppRole Role { get; set; }

    }
}
