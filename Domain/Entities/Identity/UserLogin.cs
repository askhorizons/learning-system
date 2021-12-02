using Microsoft.AspNetCore.Identity;

namespace Entities
{
    public class UserLogin : IdentityUserLogin<string>
    {
        public virtual AppUser User { get; set; }


    }
}
