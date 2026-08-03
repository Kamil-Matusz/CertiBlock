using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Queries;
using CertiBlock.Services.Users.Core.DTO;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.Handlers;

public sealed class GetAllUsersHandler(UsersDbContext dbContext) : IQueryHandler<GetAllUsers, IEnumerable<UserDto>>
{
    public async Task<IEnumerable<UserDto>> HandleAsync(GetAllUsers query)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
        
        return users.Select(user => user.AsUsersDto()).ToList();
    }
}