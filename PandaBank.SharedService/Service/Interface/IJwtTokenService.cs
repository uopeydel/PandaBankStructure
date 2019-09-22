using PandaBank.SharedService.Contract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Service.Interface
{
    public interface IJwtTokenService
    {
        string GenerateRefreshToken();
        string GenerateToken(string email, string Id);
        string GenerateToken(List<Claim> claims);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GetValueFromClaimType(ClaimsPrincipal principal, string claimType = JwtRegisteredClaimNames.Email);
    }
}
