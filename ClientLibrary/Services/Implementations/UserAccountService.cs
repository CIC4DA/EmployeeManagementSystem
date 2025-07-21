using System.Net.Http.Json;
using BaseLibrary.DTOs;
using BaseLibrary.Responses;
using ClientLibrary.Services.Contracts;
using ClientLibrary.Services.Helpers;

namespace ClientLibrary.Services.Implementations
{
    public class UserAccountService(GetHttpClient getHttpClient) : IUserAccountService
    {
        public const string AuthUrl = "api/authentication";
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            // validation
            if (user is null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            var requiredFields = new Dictionary<string, string>
                                {
                                    { nameof(user.Fullname), user.Fullname! },
                                    { nameof(user.Email), user.Email! },
                                    { nameof(user.Password), user.Password! },
                                    { nameof(user.ConfirmPassword), user.ConfirmPassword! }
                                };

            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                    throw new ArgumentException($"{field.Key} is required", field.Key);
            }

            if (user.Password != user.ConfirmPassword)
                throw new ArgumentException("Passwords do not match", nameof(user.ConfirmPassword));

            // call the API to create the user
            var httpClient = getHttpClient.GetPublicHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{AuthUrl}/register", user);
            if(!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<GeneralResponse>();
                throw new HttpRequestException($"Error creating user: {errorResponse?.Message}");
            }

            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                   ?? throw new InvalidOperationException("Failed to Create User");
        }
        
        public async Task<LoginResponse> SignInAsync(Login User)
        {
            // validation
            if (User is null)
                throw new ArgumentNullException(nameof(User), "User cannot be null");

            var requiredFields = new Dictionary<string, string>
                                {
                                    { nameof(User.Email), User.Email! },
                                    { nameof(User.Password), User.Password! }
                                };
            foreach (var field in requiredFields)
            {
                if (string.IsNullOrWhiteSpace(field.Value))
                    throw new ArgumentException($"{field.Key} is required", field.Key);
            }

            // call the API to sign in the user
            var httpClient = getHttpClient.GetPublicHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{AuthUrl}/login", User);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                throw new HttpRequestException($"Error signing in: {errorResponse?.Message}");
            }
            return await response.Content.ReadFromJsonAsync<LoginResponse>()
                   ?? throw new InvalidOperationException("Failed to Sign In");
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken Token)
        {
            var httpClient = getHttpClient.GetPublicHttpClient();
            var response = await httpClient.PostAsJsonAsync($"{AuthUrl}/refresh-token", Token);
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                throw new HttpRequestException($"Error creating user: {errorResponse?.Message}");
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>()
                   ?? throw new InvalidOperationException("Failed to Create User");
        }

        public async Task<WeatherForecast[]> GetWeatherForecasts()
        {
            var httpClient = await getHttpClient.GetPrivateHttpClient();
            var response = await httpClient.GetFromJsonAsync<WeatherForecast[]>("api/weatherforecast");
            return response!;
        }

    }

}
