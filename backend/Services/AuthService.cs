using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using MonApp.API.Models;
using BCrypt.Net;

namespace MonApp.API.Services;

public class AuthService
{
    private readonly IConfiguration _configuration;
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin" },
        new User { Id = 2, Username = "user", Email = "user@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("user123"), Role = "User" }
    };

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public LoginResponse? Login(LoginRequest request)
    {
        var user = _users.FirstOrDefault(u => u.Username == request.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        var token = GenerateJwtToken(user);
        return new LoginResponse
        {
            Token = token,
            Username = user.Username,
            Role = user.Role
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "VotreCleSecreteTresLongueEtSecuriseePourJWT123456789");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? "VotreCleSecreteTresLongueEtSecuriseePourJWT123456789");
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}
