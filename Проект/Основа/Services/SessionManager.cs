using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Blackberries.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace Blackberries.Services
{
    public static class SessionManager
    {
        public static async Task SignInAsync(HttpContext context, AuthenticationResult authenticationResult) 
        {
            if (!authenticationResult.Success) 
            {
                throw new Exception("Аутентификация не выполнена");
            }

            await context.SignOutAsync();

            var appUser = authenticationResult.AppUser!;

            var identity = new UserIdentity(appUser.Id, appUser.Email, appUser.Name, appUser.Role);

            var claimsIdentity = new ClaimsIdentity(new[] { 
                new Claim(ClaimsIdentity.DefaultNameClaimType, identity.Name!),
                new Claim(nameof(UserIdentity.Id), identity.Id.ToString()),
                new Claim(nameof(UserIdentity.Role), identity.Role.ToString()),
                new Claim(nameof(UserIdentity.Email), identity.Email),
                new Claim(nameof(UserIdentity.DisplayName), identity.DisplayName)
            }, CookieAuthenticationDefaults.AuthenticationScheme);

            var userPrincipal = new ClaimsPrincipal(claimsIdentity);

            await context.SignInAsync(userPrincipal, new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
            });

            var genericPrincipal = new GenericPrincipal(identity, null);
            genericPrincipal.AddIdentity(claimsIdentity);

            context.User = genericPrincipal;
        }

        public static async Task SignOutAsync(HttpContext context)
        {
            await context.SignOutAsync();

            context.User = null;
        }
    }
}
