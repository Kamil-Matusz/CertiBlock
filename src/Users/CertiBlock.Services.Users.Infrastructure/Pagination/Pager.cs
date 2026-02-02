namespace CertiBlock.Services.Users.Infrastructure.Pagination;

public class Pager
{
    private int _pageIndex = 1;
    private int _pageSize = 20;
    private int _totalRows;
    private int? _cachedTotalPages;

    public int PageIndex
    {
        get => Math.Min(_pageIndex, Math.Max(TotalPages, 1));
        set
        {
            var newValue = Math.Max(value, 1);
            if (_pageIndex != newValue)
            {
                _pageIndex = newValue;
            }
        }
    }

    public int PageSize
    {
        get => _pageSize;
        set
        {
            var newValue = Math.Max(value, 1);
            if (_pageSize != newValue)
            {
                _pageSize = newValue;
                _cachedTotalPages = null; // Invalidate cache
            }
        }
    }

    public int TotalRows
    {
        get => _totalRows;
        set
        {
            var newValue = Math.Max(value, 0);
            if (_totalRows != newValue)
            {
                _totalRows = newValue;
                _cachedTotalPages = null; // Invalidate cache
            }
        }
    }

    public int TotalPages
    {
        get
        {
            if (_cachedTotalPages.HasValue)
                return _cachedTotalPages.Value;

            if (_pageSize == 0)
            {
                _cachedTotalPages = 0;
                return 0;
            }

            _cachedTotalPages = (int)Math.Ceiling((double)_totalRows / _pageSize);
            return _cachedTotalPages.Value;
        }
    }

    public int Offset => (_pageIndex - 1) * _pageSize;

    public Pager() { }

    public Pager(Pager pager)
    {
        _totalRows = pager._totalRows;
        _pageIndex = pager._pageIndex;
        _pageSize = pager._pageSize;
        _cachedTotalPages = pager._cachedTotalPages;
    }

    public Pager(int pageIndex, int pageSize = 20)
    {
        TotalRows = int.MaxValue;
        PageIndex = pageIndex;
        PageSize = pageSize;
    }
}