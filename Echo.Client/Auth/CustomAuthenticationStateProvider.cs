using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Echo.Client.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorage;
        public CustomAuthenticationStateProvider(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (string.IsNullOrWhiteSpace(token)) 
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }

            var email = await _localStorage.GetItemAsync<string>("email") ?? "User";
            
            var claims = new[] 
            { 
                new Claim(ClaimTypes.Name, email), 
                new Claim(ClaimTypes.Email, email) 
            };
            
            var identity = new ClaimsIdentity(claims, "Bearer");
            var user = new ClaimsPrincipal(identity);
            
            return new AuthenticationState(user);
        }

        public void MarkUserAsAuthenticated(string email)
        {
            var claims = new[] 
            { 
                new Claim(ClaimTypes.Name, email), 
                new Claim(ClaimTypes.Email, email) 
            };
            
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            
            NotifyAuthenticationStateChanged(authState);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            
            NotifyAuthenticationStateChanged(authState);
        }
    }
}
