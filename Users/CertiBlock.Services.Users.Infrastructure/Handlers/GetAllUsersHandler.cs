using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Queries;
using CertiBlock.Services.Users.Core.DTO;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.Handlers;

internal sealed class GetAllUsersHandler : IQueryHandler<GetAllUsers, IEnumerable<UserDto>>
{
    private readonly UsersDbContext _dbContext;

    public GetAllUsersHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<UserDto>> HandlerAsync(GetAllUsers query)
    {
        var users = await _dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .Skip((query.PageIndex - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();
        
        return users.Select(user => user.AsUsersDto()).ToList();
    }
}