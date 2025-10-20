

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.Models;
using MyApp.Models.Authentication;

namespace MyApp.Services;

public class TokenGenerator : ITokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly UserManager<ApplicationUser> _userManager;

    public TokenGenerator (
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        IConfiguration configuration,
        TokenValidationParameters tokenValidationParameters
    )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;
    }
    
    public async Task<AuthResponse> GenerateJwtTokenAsync (ApplicationUser user)
    {
        return await GenerateJwtTokenAsync(user, string.Empty);
    }

    public async Task<AuthResponse> GenerateJwtTokenAsync (ApplicationUser user, string? existingRefreshToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        authClaims.AddRange(userRoles.Select(userRole => new Claim(ClaimTypes.Role, userRole)));

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]!));

        var token = new JwtSecurityToken(
            _configuration["JWT:Issuer"],
            _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        string refreshTokenString;

        if (string.IsNullOrEmpty(existingRefreshToken))
        {
            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid() + "-" + Guid.NewGuid()
            };

            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            refreshTokenString = refreshToken.Token;
        }
        else
        {
            refreshTokenString = existingRefreshToken;
        }

        return new AuthResponse
        {
            Token = jwtToken,
            RefreshToken = refreshTokenString,
            ExpiryDate = token.ValidTo
        };
    }

    public async Task<AuthResponse> VerifyAndGenerateTokenAsync (TokenRequestModel tokenRequest)
    {
        try
        {
            /*
                Checks to be performed:
                Check 1: Validate the token format
                Check 2: Validate the encryption algorithm
                Check 3: Validate the token expiry
                Check 4: Validate the existence of the refresh token
                Check 5: Validate the token id
                Check 6: Validate the refresh token expiry
                Check 7: Validate the refresh token is not revoked
            */

            //Check 1: Validate token format
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenInVerification =
                jwtHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken
                                   ?? throw new SecurityTokenException("Invalid token format");

            //Check 2: Validate algorithm
            ValidateAlgorithm(jwtSecurityToken);

            //Check 3: Validate token expiry
            ValidateTokenExpired(tokenInVerification);

            //Check 4: Validate refresh token existence
            var dbRefreshToken = ValidateRefreshTokenExists(tokenRequest.RefreshToken);

            //Check 5: Validate token id
            ValidateTokenId(tokenInVerification, dbRefreshToken);

            //Check 6: Validate refresh token expiry
            ValidateRefreshTokenExpiry(dbRefreshToken);

            //Check 7: Validate refresh token not revoked
            ValidateRefreshTokenNotRevoked(dbRefreshToken);

            //Generate new token
            var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
            return await GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);
        }
        catch (SecurityTokenExpiredException)
        {
            return await HandleExpiredAccessToken(tokenRequest);
        }
        catch (Exception error)
        {
            throw new Exception(error.Message);
        }
    }

    private static bool IsTokenExpired(ClaimsPrincipal tokenInVerification)
    {
        var expClaim = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
        if (expClaim is null) throw new SecurityTokenException("Invalid token: Missing expiry claim");

        var utcExpiryDate = long.Parse(expClaim.Value);
        var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;

        return expiryDate > DateTime.UtcNow;
    }

    private static void ValidateAlgorithm(JwtSecurityToken jwtSecurityToken)
    {
        if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenInvalidAlgorithmException("Invalid token algorithm");
    }

    private static void ValidateTokenExpired(ClaimsPrincipal tokenInVerification)
    {
        if (IsTokenExpired(tokenInVerification))
            throw new Exception("Token has not expired");
    }

    private RefreshToken ValidateRefreshTokenExists(string refreshToken)
    {
        var dbRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == refreshToken);
        if (dbRefreshToken == null)
            throw new Exception("Refresh token not found");

        return dbRefreshToken;
    }

    private static void ValidateTokenId(ClaimsPrincipal tokenInVerification, RefreshToken dbRefreshToken)
    {
        var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;
        if (dbRefreshToken.JwtId != jti)
            throw new Exception("Token does not match");
    }

    private static void ValidateRefreshTokenExpiry(RefreshToken dbRefreshToken)
    {
        if (DateTime.UtcNow > dbRefreshToken.ExpiryDate)
            throw new Exception("Refresh token has expired");
    }

    private static void ValidateRefreshTokenNotRevoked(RefreshToken dbRefreshToken)
    {
        if (dbRefreshToken.IsRevoked)
            throw new Exception("Refresh token has been revoked");
    }

    private async Task<AuthResponse> HandleExpiredAccessToken(TokenRequestModel tokenRequest)
    {
        var dbRefreshToken = ValidateRefreshTokenExists(tokenRequest.RefreshToken);

        if (dbRefreshToken.UserId is null)
            throw new Exception("Refresh token not found");

        var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
        if (user is null)
            throw new Exception("User not found");

        return await GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);
    }
}