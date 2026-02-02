using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Application.Services.Clock;
using CertiBlock.Services.Users.Core.DTO;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CertiBlock.Services.Users.Infrastructure.Auth;

internal sealed class Authenticator(IOptions<AuthOptions> options, IClock clock) : IAuthenticator
{
    private readonly string _issuer = options.Value.Issuer;
    private readonly string _audience = options.Value.Audience;
    private readonly TimeSpan _expiry = options.Value.Expiry ?? TimeSpan.FromHours(1);
    private readonly SigningCredentials _signingCredentials = new(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey)),SecurityAlgorithms.HmacSha256);
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public JwtDto CreateToken(Guid userId, string role)
    {
        var now = clock.CurrentDate();
        var expires = now.Add(_expiry);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
            new(ClaimTypes.Role, role)
        };
        
        // generating Tokens
        var jwt = new JwtSecurityToken(_issuer, _audience, claims,now, expires, _signingCredentials);
        var accessToken = _jwtSecurityTokenHandler.WriteToken(jwt);

        return new JwtDto
        {
            AccessToken = accessToken
        };
    }
}