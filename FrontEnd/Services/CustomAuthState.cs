using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using NetcodeHub.Packages.Extensions.LocalStorage;

namespace FrontEnd.Services;

public class CustomAuthState
    (ILocalStorageService localStorageService) : AuthenticationStateProvider
{
    private ClaimsPrincipal anonymous = new(new ClaimsIdentity());
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string token = await localStorageService.GetEncryptedItemAsStringAsync("token");
        if (!string.IsNullOrEmpty(token))
        {
            anonymous = SetClaimPrincipal(token);
            return await Task.FromResult(new AuthenticationState(anonymous));
        }
        return await Task.FromResult(new AuthenticationState(anonymous));
    }

    private static ClaimsPrincipal SetClaimPrincipal(string token)
    {
        try 
        {
            var handler = new JwtSecurityTokenHandler();
            var _token = handler.ReadJwtToken(token);
            var claims = _token.Claims;
            return new ClaimsPrincipal(new ClaimsIdentity(claims, "JwtAuth"));
        }
        catch { return new(new ClaimsIdentity()); }
    }

    public void NotifyAuthStateChanged()
    {
        NotifyAuthenticationStateChanged
            (Task.FromResult(new AuthenticationState(anonymous)));
    }
}
