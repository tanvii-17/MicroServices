using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mango.Services.AuthAPI.Service
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtOptions _jwtOptions;
        public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
        {
            _jwtOptions = jwtOptions?.Value ?? throw new ArgumentNullException(nameof(jwtOptions), "JWT options configuration is missing.");

            if (string.IsNullOrWhiteSpace(_jwtOptions.Secret))
            {
                throw new ArgumentException("JWT Secret key is not configured.");
            }
        }
        public string GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles)
        {
            if (applicationUser == null)
                throw new ArgumentNullException(nameof(applicationUser), "ApplicationUser cannot be null.");

            //provides methods to create and validate JWT tokens.
            var tokenHandler = new JwtSecurityTokenHandler();

            //The secret key is converted to a byte array, which is necessary for the signing credentials.
            var key = Encoding.ASCII.GetBytes(_jwtOptions.Secret);

            // Claims are key-value pairs included in the JWT payload to provide information about the user.
            var claimsList = new List<Claim>()
            { new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
              new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
              new Claim(JwtRegisteredClaimNames.Name, applicationUser.UserName)
            };

            //to add 1 or more role
            claimsList.AddRange(roles.Select(role=> new Claim(ClaimTypes.Role, role)));

            //Specifies the properties of the token, including claims, issuer, audience, expiration, and signing credentials.
            var tokenDescriptor = new SecurityTokenDescriptor
            { 
                Audience = _jwtOptions.Audience,
                Issuer = _jwtOptions.Issuer,
                Subject = new ClaimsIdentity(claimsList),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            //generate token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Serializes the token into a string format that can be returned to the client.
            return tokenHandler.WriteToken(token);
        }
    }
}
