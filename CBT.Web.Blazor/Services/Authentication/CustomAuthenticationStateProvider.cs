using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CBT.Web.Blazor.Services.Authentication
{
    public class CustomAuthenticationStateProvider  : AuthenticationStateProvider
    {
        private ClaimsPrincipal _user;

        public void SetAuthenticationState(ClaimsPrincipal user)
        {
            _user = user;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_user ?? new ClaimsPrincipal()));
        }
    }
}
