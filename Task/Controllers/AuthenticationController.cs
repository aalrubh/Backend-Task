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

namespace MyApp.Controllers
{ 
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _dbContext;

        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<ApplicationUser> _userManager;

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

                if (result is null)
                    return new JSONResponseDTO
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Message = "Invalid tokens"
                    };

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
        


        private async Task<AuthResponse> VerifyAndGenerateTokenAsync(TokenRequestModel tokenRequest)
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

                //Check 1: Validate the token format
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenInVerification = jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters,
                    out var validatedToken);
                if (validatedToken is not JwtSecurityToken jwtSecurityToken)
                    throw new SecurityTokenException("Invalid token format");

                //Check 2 : Validate the encryption algorithm
                if (!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                        StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenInvalidAlgorithmException("Invalid token algorithm");

                //Check 3: Validate the token expiry
                if (IsTokenExpired(tokenInVerification))
                {
                    throw new Exception("Token has not expired");
                }

                //Check 4:Validate the existence of the refresh token
                var dbRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);
                if (dbRefreshToken == null) throw new Exception("Refresh token not found");

                //Check 5: Validate the token id
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (dbRefreshToken.JwtId != jti) throw new Exception("Token does not match");

                //Check 6: Validate the refresh token expiry
                if (DateTime.UtcNow > dbRefreshToken.ExpiryDate) throw new Exception("Refresh token has expired");

                //Check 7: Validate the refresh token is not revoked
                if (dbRefreshToken.IsRevoked) throw new Exception("Refresh token has been revoked");

                //Finally: Generate new token
                var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);
                var newToken = GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);

                return await newToken;
            }
            catch (SecurityTokenExpiredException)
            {
                // If the token has expired, we can still use the refresh token to generate a new token
                var dbRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(x => x.Token == tokenRequest.RefreshToken);

                //Check 4:Validate the existence of the refresh token
                if (dbRefreshToken == null) throw new Exception("Refresh token not found");

                if (dbRefreshToken.UserId is null) throw new Exception("Refresh token not found");

                //Finally: Generate new token
                var user = await _userManager.FindByIdAsync(dbRefreshToken.UserId);

                if (user is null) throw new Exception("User not found");

                var newToken = GenerateJwtTokenAsync(user, tokenRequest.RefreshToken);

                return await newToken;
            }
            catch (Exception error)
            {
                throw new Exception(error.Message);
            }
        }

        private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user)
        {
            return await GenerateJwtTokenAsync(user, string.Empty);
        }

        private async Task<AuthResponse> GenerateJwtTokenAsync(ApplicationUser user, string? existingRefreshToken)
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
        
        private static bool IsTokenExpired(ClaimsPrincipal tokenInVerification)
        {
            var expClaim = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp);
            if (expClaim is null) throw new SecurityTokenException("Invalid token: Missing expiry claim");

            var utcExpiryDate = long.Parse(expClaim.Value);
            var expiryDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiryDate).UtcDateTime;

            return expiryDate > DateTime.UtcNow;
        }
        
        private SecurityToken IsTokenValidFormat(TokenRequestModel tokenRequest)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            jwtTokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters,
                out var validatedToken);

            if (validatedToken is not JwtSecurityToken)
                throw new SecurityTokenException("Invalid token format");

            return validatedToken;
        }

        
    }
}

