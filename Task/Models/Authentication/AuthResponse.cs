namespace MyApp.Models.Authentication;

public class AuthResponse
{
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiryDate { get; set; }
}