using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Project_CreateAPI.Models;
using Project_CreateAPI.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Project_CreateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IActionResult> GetAll()
        {
            var courses = await _userRepository.GetUsers();
            if (courses != null)
            {
                return Ok(courses);
            }
            return BadRequest("Can't find list courses");
        }

        [HttpGet("token")]
        [EnableQuery]
        [Authorize]
        public async Task<IActionResult> GetUserByToken()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.ReadJwtToken(headerValue.Parameter);
                var email = token.Claims.First(s => s.Type == JwtRegisteredClaimNames.Email).Value;
                var existingUser = await _userRepository.GetUserByEmail(email);
                if (existingUser != null)
                {
                    return Ok(existingUser);
                }
                return BadRequest();
            }
            return BadRequest();
        }

        [HttpPost("login")]
        [EnableQuery]
        public async Task<IActionResult> Login(User user)
        {
            var existingUser = await _userRepository.GetUserByEmail(user.Email);
            if (existingUser != null && existingUser.Password == user.Password)
            {
                var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                var issuer = configuration["Security:Issuer"];
                var audience = configuration["Security:Audience"];
                var key = Encoding.UTF8.GetBytes(configuration["Security:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                            new Claim("Id", Guid.NewGuid().ToString()),
                            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                            new Claim(JwtRegisteredClaimNames.Email, user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        }),
                    Expires = DateTime.UtcNow.AddMinutes(60),
                    Issuer = issuer,
                    Audience = audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var stringToken = tokenHandler.WriteToken(token);

                return Ok(stringToken);
            }
            return Unauthorized();
        }
    }
}
