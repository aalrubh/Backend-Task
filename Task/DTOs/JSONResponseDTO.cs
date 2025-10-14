using System.Net;

namespace MyApp.DTOs;

public class JSONResponseDTO
{
    public HttpStatusCode StatusCode { get; set; }
    public string? Message { get; set; }
}