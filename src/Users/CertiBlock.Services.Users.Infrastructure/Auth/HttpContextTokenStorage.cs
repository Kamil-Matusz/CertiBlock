using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.DTO;
using Microsoft.AspNetCore.Http;

namespace CertiBlock.Services.Users.Infrastructure.Auth;

internal sealed class HttpContextTokenStorage : ITokenStorage
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TokenKey = "jwt";

    public HttpContextTokenStorage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public void SetToken(JwtDto jwt) => _httpContextAccessor.HttpContext?.Items.TryAdd(TokenKey, jwt);


    public JwtDto GetToken()
    {
        if (_httpContextAccessor.HttpContext is null)
        {
            return null;
        }

        if (_httpContextAccessor.HttpContext.Items.TryGetValue(TokenKey, out var jwt))
        {
            return jwt as JwtDto;
        }

        return null;
    }

    public void ClearToken()
    {
        _httpContextAccessor.HttpContext?.Items.Remove(TokenKey);
    }
}