using System.Security.Claims;
using CertiBlock.Shared.DTO;

namespace CertiBlock.Shared.Helpers;

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