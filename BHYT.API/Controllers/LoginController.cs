using Azure.Core;
using BHYT.API.Models.DbModels;
using BHYT.API.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BHYT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly BHYTDbContext _context;
        private readonly IConfiguration _configuration;
        public LoginController(BHYTDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private IEnumerable<Claim> GetClaims(User user)
        {

            var roleName =  _context.Roles
                            .Where(r => r.Id == user.RoleId)
                            .Select(r => r.Name)
                            .FirstOrDefault();

            var account =  _context.Accounts.FirstOrDefault(u => u.Id == user.AccountId);

            return new List<Claim> {

                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.Name, account.Username),

            };
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            //ClientIp = HttpContext.Connection.RemoteIpAddress.ToString(),
            try
            {
                if (dto != null && !string.IsNullOrEmpty(dto.Username) && !string.IsNullOrEmpty(dto.Password))
                {
                    var account = _context.Accounts.SingleOrDefault(e => e.Username == dto.Username);
                    if(account != null)
                    {
                        var user = _context.Users.SingleOrDefault(u => u.AccountId == account.Id);
                        if (user != null)
                        {
                            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, account.Password);

                            if (isPasswordValid)
                            {
                                var authClaims = GetClaims(user);

                                (string token, DateTime expiration, string tokenId) = GenerateToken(authClaims);
                                string refreshToken = GenerateRefreshToken();

                                // save token and refresh token 
                                var refreshTokenEntity = new RefreshToken
                                {
                                    Id = Guid.NewGuid(),
                                    AccessTokenId = tokenId,
                                    AccountId = account.Id,
                                    Token = refreshToken,
                                    IsUsed = false,
                                    IsRevoked = false,
                                    IssuedAt = DateTime.UtcNow,
                                    ExpiredAt = DateTime.UtcNow.AddHours(Convert.ToInt64(_configuration["Jwt:RefreshTokenExpiryTimeInHour"]))
                                };

                                await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                                await _context.SaveChangesAsync();

                                return Ok(new
                                {
                                    Message = "Genarate token successfully",
                                    Token = new
                                    {
                                        AccessToken = token,
                                        IssuedAt = DateTime.UtcNow,
                                        ExpiredAt = expiration,
                                        RefreshToken = refreshToken,
                                    },
                                    Account = new {
                                        account.Id,
                                        account.Username,
                                    }
                                });
                            }
                            return BadRequest(new ApiResponse { Message = "invalid password !" });
                        }

                    }

                    return NotFound(new ApiResponse { Message = "Invalid credentials" });
                }

                return BadRequest(new ApiResponse { Message = "Username, Password are required" });

            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        private (string token, DateTime expire, string tokenId) GenerateToken(IEnumerable<Claim> claims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var _TokenExpiryTimeInHour = Convert.ToInt64(_configuration["Jwt:TokenExpiryTimeInHour"]);
            var _TokenExpiryTimeInSecond = Convert.ToInt64(_configuration["Jwt:TokenExpiryTimeInSecond"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                Expires = DateTime.UtcNow.AddHours(_TokenExpiryTimeInHour),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(claims)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return (tokenHandler.WriteToken(token), token.ValidTo, token.Id);
        }
        private string GenerateRefreshToken()
        {
            var random = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(random);

                return Convert.ToBase64String(random);
            }
        }

        [HttpPost("renew-token")]
        public async Task<IActionResult> RenewToken(TokenDTO dto)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = false //ko kiểm tra token hết hạn
            };

            try
            {
                //check AccessToken valid format
                var tokenVerification = jwtTokenHandler.ValidateToken(dto.AccessToken,tokenValidateParam, out SecurityToken validatedToken);
            
                //Check algorithm
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                    {
                        return Ok(new ApiResponseDTO
                        {
                            Success = false,
                            Message = "Invalid token"
                        });
                    }
                }

                //Check Accesstoken expired?
            
                var jwtToken = jwtTokenHandler.ReadJwtToken(dto.AccessToken);

                if (jwtToken.Payload.Exp is not null && jwtToken.Payload.Exp is int expUnixSeconds)
                {
                    var expDateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(expUnixSeconds);
                    var expireDate =  expDateTimeOffset.UtcDateTime;
                    if (expireDate > DateTime.UtcNow)
                    {
                        return Ok(new ApiResponse { Success = false, Message = "Access token has not yet expired" });
                    }
                }
                else
                {
                    return NotFound( new ApiResponse { Message = "type values ​​do not match" });
                }

                //Check refreshtoken exist in DB
                var storedToken = _context.RefreshTokens.FirstOrDefault(x => x.Token == dto.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse { Success = false, Message = "Refresh token does not exist" });
                }

                //Check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                    return Ok(new ApiResponse { Success = false, Message = "Refresh token has been used" });

                if (storedToken.IsRevoked)
                    return Ok(new ApiResponse { Success = false, Message = "Refresh token has been revoked" });

                if(storedToken.ExpiredAt < DateTime.UtcNow)
                {
                    //revoke refreshToken token is expired
                    storedToken.IsRevoked = true;
                    _context.Update(storedToken);
                    await _context.SaveChangesAsync();

                    return BadRequest(new ApiResponse { Success = false, Message = "The refreshToken has expired, please login again!" });
                }    

                //Check AccesstokenID == AccessTokenId in refresh token
                var jti = tokenVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.AccessTokenId != jti)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,                            
                        Message = "Token doesn't match"
                    });
                }


                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.Update(storedToken);
                await _context.SaveChangesAsync();


                var user = await _context.Users.SingleOrDefaultAsync(user => user.AccountId == storedToken.AccountId);

                if (user == null)
                {
                    return Ok(new ApiResponse
                    {
                        Success = false,
                        Message = "User not found when renew token !"
                    });
                }    
                (string token, DateTime expiration, string tokenId) = GenerateToken(GetClaims(user));
                string refreshToken = GenerateRefreshToken();

                return Ok(new ApiResponseDTO
                {
                    Success = true,
                    Message = "Genarate token successfully",
                    Data = new
                    {
                        Token = token,
                        IssuedAt = DateTime.UtcNow,
                        ExpiredAt = expiration,
                        RefreshToken = refreshToken,
                    }
                });
            }
            catch (Exception ex) {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
