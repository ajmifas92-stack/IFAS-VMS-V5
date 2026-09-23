namespace IFAS.VMS.Shared.DTOs.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public string[] Errors { get; set; } = [];

    public static ApiResponse<T> Ok(T data, string message = "")
        => new() { Success = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string message, params string[] errors)
        => new() { Success = false, Message = message, Errors = errors };
}
