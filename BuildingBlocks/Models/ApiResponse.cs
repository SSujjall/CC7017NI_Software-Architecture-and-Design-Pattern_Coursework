using System.Net;

namespace BuildingBlocks.Models;

public class ApiResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public Dictionary<string, string> Errors { get; set; }
    public HttpStatusCode StatusCode { get; set; }

    public static ApiResponse<T> Success(T data, string message, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new ApiResponse<T>()
        {
            IsSuccess = true,
            Data = data,
            Message = message,
            Errors = default,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> Failed(
        Dictionary<string, string> errors, string message,
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        return new ApiResponse<T>()
        {
            IsSuccess = false,
            Data = default,
            Message = message,
            Errors = errors,
            StatusCode = statusCode
        };
    }
}