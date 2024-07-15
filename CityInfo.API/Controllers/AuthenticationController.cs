using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography.X509Certificates;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public class AuthenticationRequestBody
        {
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        private class CityInfoUser
        {
            public int UserId { get; set; }
            public string Username { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string City { get; set; }

            public CityInfoUser(int userId, string username, string firstName, string lastName, string city)
            {
                UserId = userId;
                Username = username;
                FirstName = firstName;
                LastName = lastName;
                City = city;
            }
        }
        private readonly IConfiguration _configuration;
        public AuthenticationController(IConfiguration configuration) 
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        
        [HttpPost("authenticate")]
        public ActionResult<string> Authenticate(
            AuthenticationRequestBody authenticationRequestBody)
        {
            var user = ValidateUserCredentials(authenticationRequestBody.Username,
                authenticationRequestBody.Password);

            if (user == null)
            {
                return Unauthorized();
            }

            var securityKey = new SymmetricSecurityKey(
                Convert.FromBase64String(_configuration["Authentication:SecretForKey"]));
            var signingCredentials = new SigningCredentials(
                securityKey, SecurityAlgorithms.HmacSha256);

            var claimsForToken = new List<Claim>();
            claimsForToken.Add(new Claim("sub", user.UserId.ToString()));
            claimsForToken.Add(new Claim("given_name", user.FirstName));
            claimsForToken.Add(new Claim("family_name", user.LastName));
            claimsForToken.Add(new Claim("city", user.City));

            var jwtToken = new JwtSecurityToken(
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claimsForToken,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(1),
                signingCredentials);

            var tokenToReturn = new JwtSecurityTokenHandler()
                .WriteToken(jwtToken);

            return Ok(tokenToReturn);
        }

        private CityInfoUser ValidateUserCredentials(string? username, string? password)
        {
            //For demo purposes, assuming credentials are valid
            return new CityInfoUser(1, username ?? "",
                "Andy",
                "Sabee",
                "New York");
        }
    }
}
