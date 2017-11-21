using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Piranha.AspNetCore.Identity.EF
{
    public class IdentityAppClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityAppUser, Microsoft.AspNetCore.Identity.IdentityRole>
    {
        public IdentityAppClaimsPrincipalFactory(UserManager<IdentityAppUser> userManager, RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> roleManager, IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(IdentityAppUser user)
        {
            var principal = await base.CreateAsync(user);

            // Only do this if the user's first and last names are provided
            if (user.FirstName != null && user.LastName != null)
            {
                ((ClaimsIdentity)principal.Identity).AddClaims(new[] {
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                });
            }

            return principal;
        }

    }
}