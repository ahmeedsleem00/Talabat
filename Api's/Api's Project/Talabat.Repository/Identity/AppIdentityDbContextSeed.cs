using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.Identity;

namespace Talabat.Repository.Identity
{
    public static class AppIdentityDbContextSeed
    {
        public static async Task IdentitySeedAsync(UserManager<AppUser> _userManager)
        {
            if(_userManager.Users.Count() == 0)
            {

                var user = new AppUser()
                {
                    DisplayName = "Ahmeed Sleem",
                    Email = "ahmeedsleem8@gmail.com",
                    UserName = "Ahmed.Sleem",
                    PhoneNumber = "01157084165"
                };
                await _userManager.CreateAsync(user, password: "pa$$word");
            }

        }
    }
}
