using IglesiaBackend.Features.SystemRoles;
using IglesiaBackend.Features.Users;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IglesiaBackend.Features.Auth;

/// <summary>
/// Service responsible for generating JWT tokens
/// </summary>
public class JwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Generates a JWT token for an authenticated user
    /// </summary>
    public LoginResultDto GenerateToken(User user, SystemRole? systemRole)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var expiresAt = DateTime.UtcNow.AddHours(
            int.Parse(_configuration["Jwt:ExpiresHours"] ?? "12")
        );

        var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),

        // para AppDbContext (auditoría)
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),

        new Claim("memberId", user.MemberId.ToString())
    };

        if (systemRole != null)
        {
            claims.Add(new Claim(ClaimTypes.Role, systemRole.Name!));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        return new LoginResultDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiresAt = expiresAt
        };
    }
}
