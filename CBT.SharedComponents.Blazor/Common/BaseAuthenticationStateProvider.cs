using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using CBT.Domain.Identity;

namespace CBT.SharedComponents.Blazor.Common
{
    public class BaseAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor? httpContextAccessor;
        private readonly UserManager<User> userManager;

        public BaseAuthenticationStateProvider(
            IHttpContextAccessor? httpContextAccessor, 
            UserManager<User> userManager) 
        {
            this.httpContextAccessor = httpContextAccessor;
            this.userManager = userManager;
        }

        #region Current User

        protected ClaimsPrincipal CurrentUser
        {
            get
            {
                if (httpContextAccessor?.HttpContext != null)
                {
                    return httpContextAccessor.HttpContext.User ?? anonymousUser;
                }
                else
                {
                    return (ClaimsPrincipal?)Thread.CurrentPrincipal ?? anonymousUser;
                }
            }
            set
            {
                if (httpContextAccessor?.HttpContext != null)
                {
                    httpContextAccessor.HttpContext.User = value;
                }
                else
                {
                    try
                    {
                        AppDomain.CurrentDomain.SetThreadPrincipal(value);
                    }
                    catch { }
                }
            }
        }

        #endregion

        private readonly ClaimsPrincipal anonymousUser = new(new ClaimsIdentity());
        

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return GetState();
        }

        public async Task Login(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roles = await userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Sid, user.Id),
                    new(ClaimTypes.NameIdentifier, user.Id),
                    new(ClaimTypes.Email, user.Email!),
                    new(ClaimTypes.Name, user.UserName!)
                };
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims));
                CurrentUser = authenticatedUser;
            }

            NotifyAuthenticationStateChanged(GetState());
        }

        public void Logout()
        {
            CurrentUser = anonymousUser;
            NotifyAuthenticationStateChanged(GetState());
        }

        private Task<AuthenticationState> GetState()
        {
            return Task.FromResult(new AuthenticationState(CurrentUser));
        }
    }
}
