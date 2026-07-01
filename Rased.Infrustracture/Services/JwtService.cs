using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Rased.Core.DTO.Account;
using Rased.Core.Identity;
using Rased.Core.ServiseContracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Rased.Infrustracture.Services
{
    public class JwtService : IJWTService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // التعديل هنا: أضفنا باراميتر الـ roles لاستقبال أدوار المستخدم
        public AuthenticationResponse GenerateToken(ApplicationUser user, string? fullName = null, List<string>? roles = null)
        {
            DateTime expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:EXPIRATION_MINUTES"]));
            var issuedAt = EpochTime.GetIntDate(DateTime.UtcNow);

            // 1. تحويل الـ Claims لقائمة ديناميكية لإضافة الأدوار براحتنا
            var claimList = new List<Claim>
            {
               new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
               new Claim(JwtRegisteredClaimNames.Iat, issuedAt.ToString(CultureInfo.InvariantCulture), ClaimValueTypes.Integer64),
               new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
               new Claim(ClaimTypes.Name, fullName ?? string.Empty),
               new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
               new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };

            // 2. التعديل السحري: لو اليوزر له أدوار في الداتابيز، هنلف عليها ونحقنها جوه التوكن كـ Role Claim
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    claimList.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:KEY"]));
            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // 3. تمرير الـ Claims بعد تحويلها لمصفوفة مجدداً
            JwtSecurityToken tokenGenerator = new JwtSecurityToken(
                issuer: _configuration["Jwt:ISSUER"],
                audience: _configuration["Jwt:AUDIENCE"],
                claims: claimList.ToArray(),
                expires: expiration,
                signingCredentials: signingCredentials
            );

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string token = tokenHandler.WriteToken(tokenGenerator);

            return new AuthenticationResponse
            {
                PersonName = fullName,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Password = user.PasswordHash ?? string.Empty,
                Token = token,
                Expiration = expiration
            };
        }
    }
}