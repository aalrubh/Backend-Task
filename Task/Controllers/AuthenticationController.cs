using System.Net;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyApp.Data;
using MyApp.DTOs;
using MyApp.Models;
using MyApp.Models.Authentication;
using MyApp.Services;

namespace MyApp.Controllers;

public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    
    private readonly TokenGenerator _tokenGenerator;

    public AuthenticationController (
        UserManager<ApplicationUser> userManager,
        AppDbContext dbContext,
        IConfiguration configuration,
        TokenValidationParameters tokenValidationParameters
    )
    {
        _userManager = userManager;
        
        _tokenGenerator = new TokenGenerator(
            _userManager,
            dbContext,
            configuration,
            tokenValidationParameters
        );
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null) return BadRequest("User already exists!");
        var user = new ApplicationUser
        {
            Email = model.Email,
            UserName = model.Username,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        return Ok("User created successfully!");
    }

    [HttpPost("login")]
    public async Task<JSONResponseDTO> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = "Invalid username or password"
            };
        }
        
        var token = await _tokenGenerator.GenerateJwtTokenAsync(user);

        return new JSONResponseDTO
        {
            StatusCode = HttpStatusCode.OK,
            Message = "Login successful",
            Data = token
        };

    }

    [HttpPost("refresh-token")]
    public async Task<JSONResponseDTO> RefreshToken([FromBody] TokenRequestModel tokenRequest)
    {
        try
        {
            var result = await _tokenGenerator.VerifyAndGenerateTokenAsync(tokenRequest);

            if (string.IsNullOrEmpty(result.Token))
            {
                return new JSONResponseDTO
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Message = "Invalid tokens"
                };
            }

            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.OK,
                Message = "Token refreshed successfully",
                Data = result
            };
        }
        catch (Exception error)
        {
            return new JSONResponseDTO
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Message = error.Message
            };
        }
    }
}