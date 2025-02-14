﻿using System.Security.Claims;

using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace CBT.Web.Blazor.Services.Authentication
{
    public class TokenServerAuthenticationStateProvider(
        IJSRuntime jsRuntime, 
        JwtProvider jwtProvider) : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime = jsRuntime;
        private readonly JwtProvider _jwtProvider = jwtProvider;

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
