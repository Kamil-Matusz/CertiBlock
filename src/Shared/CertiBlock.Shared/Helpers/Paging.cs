namespace CertiBlock.Shared.Helpers;

public static class Paging
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public static (int PageIndex, int PageSize) Normalize(int pageIndex, int pageSize)
    {
        var index = Math.Max(pageIndex, 1);
        var size = pageSize < 1 ? DefaultPageSize : Math.Min(pageSize, MaxPageSize);

        return (index, size);
    }
}