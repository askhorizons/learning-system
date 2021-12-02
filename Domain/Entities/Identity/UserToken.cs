using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class UserToken : IdentityUserToken<string>
    {
        public virtual AppUser User { get; set; }
    }
}
