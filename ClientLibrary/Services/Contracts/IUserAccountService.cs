using BaseLibrary.DTOs;
using BaseLibrary.Responses;

namespace ClientLibrary.Services.Contracts
{
    public interface IUserAccountService
    {
        Task<GeneralResponse> CreateAsync(Register User);
        Task<LoginResponse> SignInAsync(Login User);
        Task<LoginResponse> RefreshTokenAsync(RefreshToken Token);
        Task<WeatherForecast[]> GetWeatherForecasts();
    }
}
