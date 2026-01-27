using CertiBlock.Services.Users.Core.DTO;
using CertiBlock.Services.Users.Core.Entities;

namespace CertiBlock.Services.Users.Infrastructure.Handlers;

public static class Mappers
{
    public static AccountDto AsAccountDto(this User entity)
        => new()
        {
            UserId = entity.UserId,
            Email = entity.Email,
            Role = entity.Role,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    
    public static UserDto AsUsersDto(this User entity)
        => new()
        {
            UserId = entity.UserId,
            Email = entity.Email,
            Role = entity.Role,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
}