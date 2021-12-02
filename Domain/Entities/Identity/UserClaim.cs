using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class UserClaim : IdentityUserClaim<string>
    {
        public virtual AppUser User { get; set; }
    }
}
