using System.Net;

namespace UserService.Helpers;

public class ServiceException : Exception
{
    public Dictionary<string, string> Errors { get; }
    public HttpStatusCode StatusCode { get; }

    public ServiceException(Dictionary<string, string> errors, HttpStatusCode statusCode)
    {
        Errors = errors;
        StatusCode = statusCode;
    }
}