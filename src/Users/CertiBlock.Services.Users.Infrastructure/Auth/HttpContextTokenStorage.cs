using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace CertiBlock.Services.Users.Infrastructure.Auth;

internal sealed class HttpContextTokenStorage(IHttpContextAccessor httpContextAccessor) : ITokenStorage
{
    private const string TokenKey = "jwt";

    public void SetToken(JwtDto jwt) => httpContextAccessor.HttpContext?.Items.TryAdd(TokenKey, jwt);
    
    public JwtDto GetToken()
    {
        if (httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        if (httpContextAccessor.HttpContext.Items.TryGetValue(TokenKey, out var jwt))
        {
            return jwt as JwtDto;
        }

        return null;
    }
}