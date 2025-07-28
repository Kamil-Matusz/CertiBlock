using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Queries;
using CertiBlock.Services.Users.Core.DTO;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.Handlers;

internal sealed class GetAccountInfoHandler : IQueryHandler<GetAccountInfo, AccountDto>
{
    private readonly UsersDbContext _dbContext;

    public GetAccountInfoHandler(UsersDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountDto> HandlerAsync(GetAccountInfo query)
    {
        var userId = query.UserId;
        var user = await _dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId);

        var accountDto = user.AsAccountDto();

        return accountDto;
    }
}