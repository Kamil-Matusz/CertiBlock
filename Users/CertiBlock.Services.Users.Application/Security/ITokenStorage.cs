using CertiBlock.Services.Users.Core.DTO;

namespace CertiBlock.Services.Users.Application.Security;

public interface ITokenStorage
{
    void SetToken(JwtDto jwt);
    JwtDto GetToken();
    void ClearToken();
}