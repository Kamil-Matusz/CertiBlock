using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Application.Queries;
using CertiBlock.Services.Users.Core.DTO;
using CertiBlock.Services.Users.Core.Exceptions;
using CertiBlock.Services.Users.Infrastructure.DAL.PostgreSQL;
using Microsoft.EntityFrameworkCore;

namespace CertiBlock.Services.Users.Infrastructure.Handlers;

public sealed class GetAccountInfoHandler(UsersDbContext dbContext) : IQueryHandler<GetAccountInfo, AccountDto>
{
    public async Task<AccountDto> HandleAsync(GetAccountInfo query)
    {
        var userId = query.UserId;
        var user = await dbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId);

        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }

        return user.AsAccountDto();
    }
}