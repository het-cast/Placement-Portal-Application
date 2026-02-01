using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PlacementPortal.BLL.Constants;
using PlacementPortal.BLL.IService;
using PlacementPortal.DAL.ViewModels;

namespace PlacementPortal.BLL.Service;

public class JWTService : IJWTService
{
    private readonly IConfiguration _configuration;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public JWTService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }
    public string GenerateJwtToken(int userId, string email, string role, int? expiryMinutes = null)
    {
        var jwtSettings = _configuration.GetSection(ConfigurationConsts.JWTSection);

        int tokenExpiryMinutes = expiryMinutes ?? int.Parse(jwtSettings["TokenExpiryMinutes"] ?? "1440");

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim(ClaimTypes.Email, email),
        new Claim(ClaimTypes.Role, role),
        new Claim("id", userId.ToString()),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(tokenExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Validates the JWT Token Passed and extracts Claims.
    public (string, string, int) ValidateToken(string token)
    {
        try
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var jwtkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtkey,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"]
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var expiryClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
            if (expiryClaim != null && DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiryClaim)) < DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException("Token has expired.");
            }

            string emailClaim = jwtToken.Claims.FirstOrDefault(e => e.Type == ClaimTypes.Email)?.Value;
            string roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            string userIdString = jwtToken.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            int userId = int.Parse(userIdString);

            return (emailClaim, roleClaim, userId);
        }
        catch (Exception ex)
        {
            throw new Exception("An error occurred while validating the token.");
        }
    }

    // Retrieves Token Data
    public TokenDataDTO GetTokenData()
    {
        var token = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];

        if (token == null)
        {
            return new TokenDataDTO()
            {
                IsTokenPresent = false,
            };
        }

        (string email, string role, int userId) = ValidateToken(token!);

        TokenDataDTO tokenData = new()
        {
            Email = email,
            Role = role,
            UserId = userId
        };

        return tokenData;
    }
}
