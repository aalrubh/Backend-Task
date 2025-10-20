using MyApp.Models;
using MyApp.Models.Authentication;

namespace MyApp.Services;

public interface ITokenGenerator
{
    Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user);
    Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user, string? existingRefreshToken);
    Task<AuthResponse> VerifyAndGenerateTokenAsync(TokenRequestModel tokenRequest);
}