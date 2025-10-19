namespace MyApp.Models.Authentication;

public class TokenRequestModel
{
    public required string Token {get; set;}
    public required string RefreshToken {get; set; }
}