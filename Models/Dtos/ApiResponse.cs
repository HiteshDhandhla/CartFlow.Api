namespace CartFlow.Api.Models.Dtos;

public sealed record ApiResponse<T>(bool Success, string Message, T? Data)
{
    public static ApiResponse<T> Ok(T data, string message = "Request completed successfully.") =>
        new(true, message, data);

    public static ApiResponse<T> Fail(string message) => new(false, message, default);
}
