namespace Anipi.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; } = true;
    public string Message { get; set; } = "Success";
    public T? Data { get; set; }
}

public class PaginatedResponse<T> : ApiResponse<List<T>>
{
    public PaginationMetadata Pagination { get; set; } = new();
}

public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}

public class AnimeQueryParameters
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;
    
    public int Page { get; set; } = 1;
    
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
    
    public string? Title { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
    public int? Year { get; set; }
    public string? Season { get; set; }
    public string? Tag { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
