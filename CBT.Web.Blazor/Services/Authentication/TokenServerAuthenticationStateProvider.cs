using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace CBT.Web.Blazor.Services.Authentication
{
    public class TokenServerAuthenticationStateProvider: AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly JwtProvider _jwtProvider;

        public TokenServerAuthenticationStateProvider(IJSRuntime jsRuntime, JwtProvider jwtProvider)
        {
            _jsRuntime = jsRuntime;
            _jwtProvider = jwtProvider;
        }

        public async Task<string> GetTokenAsync()
             => await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");

        public async Task SetTokenAsync(string token)
        {
            if (token == null)
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
            }
            else
            {
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await GetTokenAsync();
            var identity = string.IsNullOrEmpty(token)
                ? new ClaimsIdentity()
                : new ClaimsIdentity(_jwtProvider.ParseToken(token), "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }

}
