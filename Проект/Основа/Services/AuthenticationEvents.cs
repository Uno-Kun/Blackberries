using Microsoft.AspNetCore.Authentication.Cookies;
using Blackberries.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace Blackberries.Services
{
    public class AuthenticationEvents: CookieAuthenticationEvents
    {
        public AuthenticationEvents() 
        {
            this.OnValidatePrincipal = context => {
                if (context.Principal != null && context.Principal.Identity is ClaimsIdentity claimsIdentity) 
                {
                    var userId = claimsIdentity.Claims.FirstOrDefault(x => x.Type == nameof(UserIdentity.Id))?.Value;
                    var email = claimsIdentity.Claims.FirstOrDefault(x => x.Type == nameof(UserIdentity.Email))?.Value;
                    var name = claimsIdentity.Claims.FirstOrDefault(x => x.Type == nameof(UserIdentity.DisplayName))?.Value;
                    var role = claimsIdentity.Claims.FirstOrDefault(x => x.Type == nameof(UserIdentity.Role))?.Value;

                    var userIdentity = new UserIdentity(long.Parse(userId), email, name, Enum.Parse<UserRole>(role));
                    var userPrincipal = new GenericPrincipal(userIdentity, null);
                    userPrincipal.AddIdentities(context.Principal.Identities);

                    context.Principal = userPrincipal;
                    return Task.CompletedTask;
                }

                context.Principal = null;
                return Task.CompletedTask;
            };
        }
    }
}
