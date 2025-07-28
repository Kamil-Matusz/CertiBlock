using CertiBlock.Services.Users.Core.DTO;

namespace CertiBlock.Services.Users.Application.Security;

public interface IAuthenticator
{
    JwtDto CreateToken(Guid userId, string role);
}