using Microsoft.Extensions.Configuration;
using Moq; 
using PandaBank.SharedService.Service.Implement;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Xunit;

namespace PandaBank.UnitTest.SharedService.JwtToken
{

    public class GenerateTokenTest
    {
        private JwtTokenService jwtService;

        private readonly Mock<IConfiguration> _configMocking;
        public GenerateTokenTest()
        {
            var mock = new MockRepository(MockBehavior.Default);
            _configMocking = mock.Create<IConfiguration>();

            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Key")])
                .Returns("PandaBank AuthenticationKey For JWT Token");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:ExpiredAt")])
               .Returns("30");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Issuer")])
              .Returns("PandaBankIssuer");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Audience")])
              .Returns("PandaBankAudience");

            jwtService = new JwtTokenService(_configMocking.Object);
        }

        [Fact]
        public void GenerateTokenSuccess()
        {
            var email = "test@test.test";
            string id = "1";
            var token = jwtService.GenerateToken(email, id);
            var principal = jwtService.GetPrincipalFromExpiredToken(token);
            var IdFromClaims = principal.Claims.Where(w => w.Type.Contains(JwtRegisteredClaimNames.NameId))
                .Select(s => s.Value).FirstOrDefault();
            var emailFromClaims = principal.Claims.Where(w => w.Type.Contains(JwtRegisteredClaimNames.Email))
               .Select(s => s.Value).FirstOrDefault();

            Assert.Equal(id, IdFromClaims);
            Assert.Equal(email, emailFromClaims);
        }


        [Theory]
        [InlineData(null, null)]
        [InlineData("", "")]
        [InlineData("/", "!")]
        [InlineData(" ", " ")]
        [InlineData(" @ ", "e1")]
        [InlineData("@", "@")]
        [InlineData("a @a.a", "")]
        [InlineData("a@a", "")]
        [InlineData("a@a .com", "")]
        [InlineData("true", "")]
        [InlineData("a@a.a", "")]
        public void GenerateTokenUnSuccess(string email, string Id)
        {
            var token = jwtService.GenerateToken(email, Id);
            Assert.Empty(token);
        }

    }
}
