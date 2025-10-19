using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.DTOs;
using MyApp.Models;
using MyApp.Models.Authentication;

namespace MyApp.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;
    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly TokenValidationParameters _tokenValidationParameters;
    

    public AuthenticationController(
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

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
        {
            return BadRequest("User already exists!");
        }
        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok("User created successfully!");
    }
    
    [HttpPost("login")]
    public async Task<JSONResponseDTO> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var token = await GenerateJwtTokenAsync(user);
            
            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Login successful",
                Data = token
            };
        }

        return new JSONResponseDTO
        {
            StatusCode = HttpStatusCode.InternalServerError,
            Message = "Invalid username or password"
        };
    }

    [HttpPost("refresh-token")]
    public async Task<JSONResponseDTO> RefreshToken([FromBody] TokenRequestModel tokenRequest)
    {
        try
        {
            var result = await VerifyAndGenerateTokenAsync(tokenRequest);
            
            if (result == null)
            {
                return new JSONResponseDTO()
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid tokens"
                };
            }
            
            return new JSONResponseDTO()
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Token refreshed successfully",
                Data = result
            };
        }
        catch (Exception error)
        {
            return new JSONResponseDTO()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = error.Message
            };
        }
    }
    
    private async Task<AuthResponse> VerifyAndGenerateTokenAsync (TokenRequestModel tokenRequest) {
        try
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            //Check 1
            var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out var validatedToken);

            //Check 2
            if (validatedToken is JwtSecurityToken jwtSecurityToken)
            {
                var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                if (result == false)
                {
                    return null;
                }
            }

            //Check 3
            var utcExpiryDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp)?.Value);
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;

            if (expiryDate > DateTime.UtcNow)
            {
                throw new Exception("Token has not expired");
            }

            //Check 4
            var dbRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);
            if (dbRefreshToken == null)
            {
                throw new Exception("Refresh token not found");
            }
            else
            {
                //Check 5
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (dbRefreshToken.JwtId != jti)
                {
                    throw new Exception("Token does not match");
                }

                //Check 6
                if (DateTime.UtcNow > dbRefreshToken.ExpiryDate)
                {
                    throw new Exception("Refresh token has expired");
                }

                //Check 7
                if (dbRefreshToken.IsRevoked)
                {
                    throw new Exception("Refresh token has been revoked");
                }
            }

            //Generate new token
            var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
            var newToken = GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);

            return await newToken;
        }
        catch (SecurityTokenExpiredException)
        {
            var dbRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);
            if (dbRefreshToken == null)
            {
                throw new Exception("Refresh token not found");
            }
            
            var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
            var newToken = GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);
            
            return await newToken;
        }
        catch
        {
            throw;
        }
    }

    private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user)
    {
        return await GenerateJwtTokenAsync(user, string.Empty);
    }
    
    private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user, string existingRefreshToken)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var authClaims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));

        var token = new JwtSecurityToken(
            _configuration["JWT:Issuer"],
            _configuration["JWT:Audience"],
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        var refreshToken = new RefreshToken();
        
        if (string.IsNullOrEmpty(existingRefreshToken))
        {
            // Reuse existing refresh token
            
            refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                AddedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddMonths(6),
                Token = Guid.NewGuid() + "-" + Guid.NewGuid()
            };
        }
        
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new AuthResponse
        {
            Token = jwtToken,
            RefreshToken = string.IsNullOrEmpty(existingRefreshToken) ? refreshToken.Token : existingRefreshToken,
            ExpiryDate = token.ValidTo
        };
    }
}