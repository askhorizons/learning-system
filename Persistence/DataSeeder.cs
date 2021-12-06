using Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataSeeder
    {
        public static async Task SeedData(DataContext context, UserManager<AppUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var users = new List<AppUser>
                {
                    new AppUser {
                        FirstName = "Salman",
                        LastName = "Malik",
                        Email = "salman@askhorizons.com",
                        Created = DateTime.Now,
                        UserName = "6110117675441"
                    }
                };

                foreach (var user in users)
                {
                    await userManager.CreateAsync(user, "Pa$$word");
                }
            }
        }
    }
}
