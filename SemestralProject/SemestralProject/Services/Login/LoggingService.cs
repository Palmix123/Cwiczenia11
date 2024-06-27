using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Cwiczenia10.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SemestralProject.Data;
using SemestralProject.Data.LoginDTOs;
using SemestralProject.Models;

namespace SemestralProject.Services.Login;

public class LoggingService(ProjectContext context) : ILoggingService
{
    private ProjectContext Context { get; set; } = context;
    
    public async Task RegisterUser(LoginRequest model)
    {
        var hashedPasswordAndSalt = Logger.GetHashedPasswordAndSalt(model.Password);

        var user = new User()
        {
            Login = model.Login,
            Password = hashedPasswordAndSalt.Item1,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = Logger.GenerateRefreshToken()
        };

        Context.Users.Add(user);
        await Context.SaveChangesAsync();
    }
    
    public async Task<Tuple<string, string>> LoginUser(LoginRequest request, IConfiguration configuration)
    {
        var user = await Context.Users.SingleOrDefaultAsync(u => u.Login == request.Login);

        if (user == null || user.Password != Logger.GetHashedPasswordWithSalt(request.Password, user.Salt))
        {
            throw new Exception("Invalid credentials.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "admin")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(30)
        );

        user.RefreshToken = Logger.GenerateRefreshToken();
        await Context.SaveChangesAsync();
        var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new Tuple<string, string>(accessToken, user.RefreshToken);
    }
    
    public async Task<Tuple<string, string>> RefreshAccessToken(string refreshToken, IConfiguration configuration)
    {
        var user = await Context.Users.SingleOrDefaultAsync(u => u.RefreshToken == refreshToken);

        if (user == null)
        {
            throw new Exception("Invalid refresh token");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Login),
            new Claim(ClaimTypes.Role, "admin")
        };

        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        JwtSecurityToken token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            signingCredentials: creds,
            expires: DateTime.UtcNow.AddMinutes(30)
        );

        user.RefreshToken = Logger.GenerateRefreshToken();
        await Context.SaveChangesAsync();
        string accessToken = new JwtSecurityTokenHandler().WriteToken(token);
        return new Tuple<string, string>(accessToken, user.RefreshToken);
    }
}