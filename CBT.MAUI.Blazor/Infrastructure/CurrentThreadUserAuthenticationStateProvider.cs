using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Components.Authorization;

using CBT.SharedComponents.Blazor.Common;

namespace CBT.MAUI.Blazor.Infrastructure
{
    public class CurrentThreadUserAuthenticationStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync() =>
            Task.FromResult(
                new AuthenticationState(Thread.CurrentPrincipal as ClaimsPrincipal ??
                    new ClaimsPrincipal(new ClaimsIdentity())));
    }
}
