using System.Net;
using System.Text.Json;


namespace MyApp.Middleware;

public class ExceptionHandler
{
    // Middleware implementation goes here
    private readonly RequestDelegate _next;
    
    public ExceptionHandler(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Error] {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            
            var response = JsonSerializer.Serialize(new
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "An internal server error occurred."
            });
            
            await context.Response.WriteAsync(response);
        }
    }
}