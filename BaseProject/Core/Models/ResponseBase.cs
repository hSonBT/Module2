using Azure;

namespace BaseProject.Core.Models;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T"></typeparam>
public class ResponseBase<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }

    public static ResponseBase<T> SuccessResponse(T data, string message = "Success") => new()
    {
        Success = true, Data = data, Message = message
    };

    public static ResponseBase<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null) => new()
    {
        Success = false, Message = message, Errors = errors
    };

    public static ResponseBase<T> NotFoundResponse(string message = "Resource not found") => new()
    {
        Success = false, Message = message
    };
}