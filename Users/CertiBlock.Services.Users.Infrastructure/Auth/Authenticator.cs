using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Application.Services.Clock;
using CertiBlock.Services.Users.Core.DTO;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CertiBlock.Services.Users.Infrastructure.Auth;

internal sealed class Authenticator : IAuthenticator
{
    private readonly IClock _clock;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TimeSpan _expiry;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public Authenticator(IOptions<AuthOptions> options, IClock clock)
    {
        _clock = clock;
        _issuer = options.Value.Issuer;
        _audience = options.Value.Audience;
        _expiry = options.Value.Expiry ?? TimeSpan.FromHours(1);
        _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Value.SigningKey)),SecurityAlgorithms.HmacSha256);
    }
    
    public JwtDto CreateToken(Guid userId, string role)
    {
        var now = _clock.CurrentDate();
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