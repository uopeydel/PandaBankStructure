using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Service.Interface;
using PandaBank.SharedService.Validate;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Service.Implement
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtTokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(List<Claim> claims)
        {
            var Id = claims.Where(w => w.Type.Contains(JwtRegisteredClaimNames.NameId))
                .Select(s => s.Value).FirstOrDefault();
            var email = claims.Where(w => w.Type.Contains(JwtRegisteredClaimNames.Email))
               .Select(s => s.Value).FirstOrDefault();

            return GenerateToken(email, Id);
        }



        public string GenerateToken(string email, string Id)
        {
            var emailValid = PandaValidator.EmailIsValid(email);
            var idIsValid = int.TryParse(Id, out _);
            if (!emailValid || !idIsValid)
            {
                return string.Empty;
            }

            var claims = new[] {
                new Claim (JwtRegisteredClaimNames.NameId ,Id),
                new Claim (JwtRegisteredClaimNames.Email ,email),
                new Claim (JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiredAt = double.Parse(_configuration["JWT:ExpiredAt"]);
            var expiredTokenDate = DateTime.Now.AddMinutes(expiredAt);
            var token = new JwtSecurityToken(
                _configuration["JWT:Issuer"],
                _configuration["JWT:Audience"],
                claims,
                expires: expiredTokenDate,
                signingCredentials: credential);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["JWT:Issuer"],
                ValidAudience = _configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                var token = Convert.ToBase64String(randomNumber);
                return token;
            }
        }

        public string GetValueFromClaimType(ClaimsPrincipal principal, string claimType = JwtRegisteredClaimNames.Email)
        {
            var value = principal.Claims.Where(w => w.Type.Contains(claimType))
              .Select(s => s.Value).FirstOrDefault();
            return value;
        }
    }
}
