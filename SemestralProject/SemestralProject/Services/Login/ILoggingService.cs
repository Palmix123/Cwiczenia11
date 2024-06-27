using SemestralProject.Data.LoginDTOs;

namespace SemestralProject.Services.Login;

public interface ILoggingService
{
    Task RegisterUser(LoginRequest model);
    Task<Tuple<string, string>> LoginUser(LoginRequest request, IConfiguration configuration);
    Task<Tuple<string, string>> RefreshAccessToken(string refreshToken, IConfiguration configuration);
}