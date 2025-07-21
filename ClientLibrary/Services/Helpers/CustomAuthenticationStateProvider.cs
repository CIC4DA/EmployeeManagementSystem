using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using BaseLibrary.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

// What it does is, it provides a custom authentication state provider for the Blazor application.
// So that we can keep the roles in check, and show the information for that role oonly


namespace ClientLibrary.Services.Helpers
{
    public class CustomAuthenticationStateProvider(LocalStorageService localStorageService) : AuthenticationStateProvider
    {
        private readonly ClaimsPrincipal anonymous = new (new ClaimsIdentity());
        
        // anytime we are moving form page 1 to 2, this method will be called
        // to check if the user is authenticated or not
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        { 
            var stringToken = await localStorageService.GetToken();
            if(string.IsNullOrEmpty(stringToken)) return await Task.FromResult(new AuthenticationState(anonymous));

            var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
            if (deserializedToken == null) return await Task.FromResult(new AuthenticationState(anonymous));

            var getUserClaims = DecryptToken(deserializedToken.Token!);
            if(getUserClaims == null) return await Task.FromResult(new AuthenticationState(anonymous)); 

            var claimsPrincipal = SetClaimPrincipal(getUserClaims);
            return await Task.FromResult(new AuthenticationState(claimsPrincipal));
        }

        // We will call this to update the authentication state
        public async Task UpdateAuthenticationState(UserSession userSession)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            if (userSession.Token != null || userSession.RefreshToken != null)
            {
                var serializeSession = Serializations.SerializeObj(userSession);
                await localStorageService.SetToken(serializeSession);
                var getUserClaims = DecryptToken(userSession.Token!);
                claimsPrincipal = SetClaimPrincipal(getUserClaims);
            }
            else
            {
                await localStorageService.RemoveToken();
            }
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }


        // ----------------- Helpers -----------------
        private static CustomUserClaims DecryptToken(string jwtToken)
        {
            if (string.IsNullOrEmpty(jwtToken))
                return new CustomUserClaims();

            //var handler = new JwtSecurityTokenHandler();

            //if (!handler.CanReadToken(jwtToken))
            //{
            //    Console.WriteLine("JWT Handler says token is invalid");
            //    return new CustomUserClaims();
            //}

            //var token = handler.ReadJwtToken(jwtToken);

            //This was giving error, so switcched to manual decoding

            try
            {
                var parts = jwtToken.Split('.');
                if (parts.Length != 3)
                    return new CustomUserClaims();

                var payload = parts[1];

                switch (payload.Length % 4)
                {
                    case 2: payload += "=="; break;
                    case 3: payload += "="; break;
                }

                var payloadBytes = Convert.FromBase64String(payload);
                var payloadJson = System.Text.Encoding.UTF8.GetString(payloadBytes);

                using var document = JsonDocument.Parse(payloadJson);

                var userId = string.Empty;
                var name = string.Empty;
                var email = string.Empty;
                var role = string.Empty;

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    switch (property.Name)
                    {
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier":
                            userId = property.Value.GetString() ?? string.Empty;
                            break;
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name":
                            name = property.Value.GetString() ?? string.Empty;
                            break;
                        case "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress":
                            email = property.Value.GetString() ?? string.Empty;
                            break;
                        case "http://schemas.microsoft.com/ws/2008/06/identity/claims/role":
                            role = property.Value.GetString() ?? string.Empty;
                            break;
                    }
                }

                return new CustomUserClaims(userId, name, email, role);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decrypting token: {ex.Message}");
                return new CustomUserClaims();
            }
        }

        private static ClaimsPrincipal SetClaimPrincipal(CustomUserClaims userClaims)
        {
            if (userClaims.Email is null) return new ClaimsPrincipal();

            return new ClaimsPrincipal(new ClaimsIdentity(
                new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, userClaims.Id),
                    new Claim(ClaimTypes.Name, userClaims.Name),
                    new Claim(ClaimTypes.Email, userClaims.Email),
                    new Claim(ClaimTypes.Role, userClaims.Role)
                }, "JwtAuth"));
        }
    }
}
