using System.Net;
using BaseLibrary.DTOs;
using ClientLibrary.Services.Contracts;

// This implements a middleware in each call for token check
namespace ClientLibrary.Services.Helpers
{
    public class CustomHttpHandler(GetHttpClient getHttpClient, LocalStorageService localStorageService, IUserAccountService userAccountService) : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync (HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool loginUrl = request.RequestUri.AbsoluteUri.Contains("login");
            bool registerUrl = request.RequestUri.AbsoluteUri.Contains("register");
            bool refreshTokenUrl = request.RequestUri.AbsoluteUri.Contains("refresh-token");

            // we return, because we dont need token in these apis
            if(loginUrl || registerUrl || refreshTokenUrl) return await base.SendAsync(request, cancellationToken);

            // now we send the request
            var result = await base.SendAsync(request, cancellationToken);
            if(result.StatusCode == HttpStatusCode.Unauthorized)
            {
                // if no token present, return unauthorized
                var stringToken = await localStorageService.GetToken();
                if (stringToken == null) return result;

                // Check if the headers contains the token
                string token = string.Empty;
                try { token = request.Headers.Authorization!.Parameter!; }
                catch (Exception ex) { Console.WriteLine($"Error reading authorization header: {ex.Message}"); token = string.Empty; }

                // deserializing token from local storage
                var deserializedToken = Serializations.DeserializeJsonString<UserSession>(stringToken);
                if (deserializedToken is null) return result;

                // if no authorization header in the request, we add it, and send agaain
                if (string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", deserializedToken.Token);
                    return await base.SendAsync(request, cancellationToken);
                }

                // if the token is expired, we call refresh token and generate a new token
                var newJwtToken = await GetRefreshedToken(deserializedToken.RefreshToken!);
                if (string.IsNullOrEmpty(newJwtToken)) return result;

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", newJwtToken);
                return await base.SendAsync(request, cancellationToken);

            }
            return result;
        }

        private async Task<string> GetRefreshedToken(string refreshToken)
        {
            var response = await userAccountService.RefreshTokenAsync(new RefreshToken() { Token = refreshToken });
            string serializedToken = Serializations.SerializeObj(new UserSession() { Token = response.Token, RefreshToken = response.RefreshToken });
            await localStorageService.SetToken(serializedToken); 
            return response.Token;
        }
    }
}
