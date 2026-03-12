using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rased.Core.DTO.Account;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rased.Core.Servies
{
    public class JwtService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public AuthenticationResponse GenerateToken(ApplicationUser user)
        {
            // Calculate the token expiration time by adding a specified number of minutes to the current UTC time.
            // The number of minutes is retrieved from the configuration settings
            DateTime expiration = DateTime.UtcNow.AddMinutes
                (Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
            // Create an array of claims to be included in the JWT token.
            // Claims are pieces of information about the user that are encoded in the token.
            Claim[] claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), // user id
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, user.FullName ?? string.Empty),
               new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
               new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };
            // Create a symmetric security key using the secret key specified in the configuration settings.
            SymmetricSecurityKey securityKey = new 
                SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:KEY"])
                );

            // Create signing credentials using the security key and specifying the hashing algorithm (HMAC SHA256 in this case).
            SigningCredentials signingCredentials = new 
                SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create a JwtSecurityToken object using the specified issuer, audience, claims, expiration time, and signing credentials.
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer: _configuration["Jwt:ISSUER"],
                audience: _configuration["Jwt:AUDIENCE"],
                claims: claims,
                expires: expiration,
                signingCredentials: signingCredentials
            );

            // Create a JwtSecurityTokenHandler to write the token as a string.
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            // Return an AuthenticationResponse object containing the user's name, phone number, generated token, and expiration time.
            return new AuthenticationResponse
            {
                PersonName = user.FullName,
                Email= user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Password = user.PasswordHash ?? string.Empty,
                Token = token,
                Expiration = expiration
            };

        }
    }
}
