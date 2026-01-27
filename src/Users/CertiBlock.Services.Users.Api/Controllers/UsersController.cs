using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Commands;
using CertiBlock.Services.Users.Application.Queries;
using CertiBlock.Services.Users.Application.Security;
using CertiBlock.Services.Users.Core.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CertiBlock.Services.Users.Api.Controllers;

public class UsersController(ICommandHandler<SignUp> signUpHandler, ICommandHandler<SignIn> signInHandler,
    ICommandHandler<DeleteUserAccount> deleteAccountHandler, ICommandHandler<ChangeUserRole> changeUserRoleHandler,
    IQueryHandler<GetAccountInfo, AccountDto> getAccountInfo, IQueryHandler<GetAllUsers, IEnumerable<UserDto>> getAllUsersHandler,
    ICommandHandler<ChangeUserPassword> changeUserPasswordHandler, ITokenStorage tokenStorage) : BaseController
{
    [HttpPost("signUp")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SignUp(SignUp command)
    {
        command = command with {UserId = Guid.NewGuid()};
        await signUpHandler.HandlerAsync(command);
        
        var user = await getAccountInfo.HandlerAsync(new GetAccountInfo() {UserId = command.UserId});
        return Ok(user);
    }
    
    [HttpPost("signIn")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JwtDto>> SignIn(SignIn command)
    {
        await signInHandler.HandlerAsync(command);
        var jwt = tokenStorage.GetToken();
        return Ok(jwt);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> GetUser(Guid userId)
    {
        var user = await getAccountInfo.HandlerAsync(new GetAccountInfo() {UserId = userId});
        return Ok(user);
    }
    
    [Authorize]
    [HttpGet("myAccount")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountDto>> AccountInfo()
    {
        if (string.IsNullOrWhiteSpace(User.Identity?.Name))
        {
            return NotFound();
        }
        
        var userId = Guid.Parse(User.Identity?.Name);
        var user = await getAccountInfo.HandlerAsync(new GetAccountInfo() {UserId = userId});

        return Ok(user);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteUserAccount(Guid userId)
    {
        await deleteAccountHandler.HandlerAsync(new DeleteUserAccount(userId));
        return NoContent();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{userId:guid}/changeUserRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangeUserRole(Guid userId, ChangeUserRole command)
    {
        await changeUserRoleHandler.HandlerAsync(command with { UserId = userId, Role  = command.Role });
        return Ok();
    }
    
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] GetAllUsers query)
        => Ok(await getAllUsersHandler.HandlerAsync(query));
    
    [Authorize]
    [HttpPut("changePassword")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ChangePassword(ChangeUserPassword command)
    {
        var userId = Guid.Parse(User.Identity?.Name);
        await changeUserPasswordHandler.HandlerAsync(command with { UserId = userId, Password  = command.Password });
        return NoContent();
    }
}