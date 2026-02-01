namespace PlacementPortal.DAL.ViewModels;

public class ApiResponseDTO<T>
{
    public bool Success { get; set; } = false;
    public string? Message { get; set; }
    public T? Data { get; set; }

    public ApiResponseDTO(bool success, string message, T? data = default)
    {
        Success = success;
        Message = message;
        Data = data;
    }
}

public static class ApiResponse
{
    public static ApiResponseDTO<T> Success<T>(string message, T? data = default) =>
        new(true, message, data);

    public static ApiResponseDTO<T> Fail<T>(string message, T? data = default) =>
        new(false, message, data);
}

