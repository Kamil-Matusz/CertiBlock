using System.Security.Claims;
using CertiBlock.Services.Certificates.Core.DTO;

namespace CertiBlock.Services.Certificates.Core.Helpers;

public static class UserContextProvider
{
    public static UserContext FromClaimsPrincipal(ClaimsPrincipal user)
    {
        return new UserContext
        {
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            Role = user.FindFirst(ClaimTypes.Role)?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value
        };
    }
}