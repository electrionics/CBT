using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

using CBT.Domain.Identity;

namespace CBT.SharedComponents.Blazor.Common
{
    public class BaseAuthenticationStateProvider(
        IHttpContextAccessor? httpContextAccessor,
        UserManager<User> userManager) : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor? _httpContextAccessor = httpContextAccessor;
        private readonly UserManager<User> _userManager = userManager;

        #region Current User

        protected ClaimsPrincipal CurrentUser
        {
            get
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    return _httpContextAccessor.HttpContext.User ?? anonymousUser;
                }
                else
                {
                    return (ClaimsPrincipal?)Thread.CurrentPrincipal ?? anonymousUser;
                }
            }
            set
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    _httpContextAccessor.HttpContext.User = value;
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
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                var roles = await _userManager.GetRolesAsync(user);

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
