namespace PowerTaskManager.DTOs;

public class PagedResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}

public class ApiResponseDto<T>
{
    public bool IsSuccess { get; set; } = true;
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    
    public static ApiResponseDto<T> Success(T data, string message = "")
    {
        return new ApiResponseDto<T>
        {
            IsSuccess = true,
            Message = message,
            Data = data
        };
    }
    
    public static ApiResponseDto<T> Failure(string message, List<string>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            IsSuccess = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}