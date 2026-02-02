using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Core.DTO;

namespace CertiBlock.Services.Users.Application.Queries;

public class GetAccountInfo : IQuery<AccountDto>
{
    public Guid UserId { get; set; }
}