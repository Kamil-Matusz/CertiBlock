using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Core.DTO;

namespace CertiBlock.Services.Users.Application.Queries;

public class GetAllUsers : IQuery<IEnumerable<UserDto>>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
}