using CertiBlock.Services.Users.Application.Abstractions;
using CertiBlock.Services.Users.Core.DTO;

namespace CertiBlock.Services.Users.Application.Queries;

public class GetAllUsers : IQuery<IEnumerable<UserDto>>
{
    private const int MaxPageSize = 100;

    private int _pageIndex = 1;
    private int _pageSize = 20;

    public int PageIndex
    {
        get => _pageIndex;
        set => _pageIndex = Math.Max(value, 1);
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Math.Clamp(value, 1, MaxPageSize);
    }
}