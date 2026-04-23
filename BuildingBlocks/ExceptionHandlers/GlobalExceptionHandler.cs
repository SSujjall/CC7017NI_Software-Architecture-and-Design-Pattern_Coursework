using System.Net;
using System.Text.Json;
using BuildingBlocks.Models;
using Microsoft.AspNetCore.Http;

namespace UserService.Helpers;

public class GlobalExceptionHandler
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleException(context, ex);
        }
    }

    private static Task HandleException(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        if (ex is ServiceException serviceEx)
        {
            context.Response.StatusCode = (int)serviceEx.StatusCode;
            var businessExMessage = "Request failed.";
            var businessStatusCode = context.Response.StatusCode;
            
            var businessRes = ApiResponse<string>.Failed(
                serviceEx.Errors,
                businessExMessage,
                (HttpStatusCode)businessStatusCode
            );
            
            var json = JsonSerializer.Serialize(
                businessRes, 
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            return context.Response.WriteAsync(json);
        }
        
        // Unknown exception
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var errors = new Dictionary<string, string>
        {
            { "SystemError", ex.Message }
        };
        var message = "An unexpected error occurred. Please try again later.";
        var statusCode = context.Response.StatusCode;

        var errorRes = ApiResponse<string>.Failed(
            errors,
            message,
            (HttpStatusCode)statusCode
        );

        var jsonResponse = JsonSerializer.Serialize(
            errorRes, 
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
        return context.Response.WriteAsync(jsonResponse);
    }
}