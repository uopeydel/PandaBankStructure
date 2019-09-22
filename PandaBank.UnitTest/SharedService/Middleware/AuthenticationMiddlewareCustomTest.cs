using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using PandaBank.SharedService.Middleware;
using PandaBank.SharedService.Service.Implement;
using PandaBank.SharedService.Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PandaBank.UnitTest.SharedService.Middleware
{

    public class AuthenticationMiddlewareCustomTest
    {
        private AuthenticationMiddlewareCustom authMiddleware;

        private readonly Mock<RequestDelegate> _next;
        private readonly JwtTokenService jwtService;
        private readonly Mock<IConfiguration> _configMocking;
        private readonly Mock<HttpContext> httpc;
        private readonly MockRepository mock;
        public AuthenticationMiddlewareCustomTest()
        {
            mock = new MockRepository(MockBehavior.Default);
            _configMocking = mock.Create<IConfiguration>();

            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Key")])
                .Returns("PandaBank AuthenticationKey For JWT Token");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:ExpiredAt")])
                .Returns("30");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Issuer")])
                .Returns("PandaBankIssuer");
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "JWT:Audience")])
                .Returns("PandaBankAudience");

            var pandaSecureKey = "pandaSecureKey";
            _configMocking.SetupGet(x => x[It.Is<string>(s => s == "SecureKey:Self")])
                .Returns("pandaSecureKey");

            jwtService = new JwtTokenService(_configMocking.Object);

            var token = jwtService.GenerateToken("test@test.test", "1");

            var headDic = new HeaderDictionary();
            headDic.Add("Authorization", $"Bearer {token}");
            headDic.Add("Panda", pandaSecureKey);

            var httpr = mock.Create<HttpRequest>();
            httpr.Setup(s => s.Headers)
                .Returns(headDic);

            var testBody = "{ 'body' : 'test' }";
            var requestData = Encoding.UTF8.GetBytes(testBody);
            var stream = new MemoryStream(requestData);

            httpr.Setup(s => s.Body)
                            .Returns(stream);
            var ps = new PathString("/api");
            httpr.Setup(s => s.Path)
                           .Returns(ps);

            httpc = mock.Create<HttpContext>();
            httpc.Setup(s => s.Request)
                .Returns(httpr.Object);

            _next = mock.Create<RequestDelegate>();
            _next.Setup(s => s(It.IsAny<HttpContext>()));

            authMiddleware = new AuthenticationMiddlewareCustom(_next.Object, jwtService, _configMocking.Object);
        }



        [Fact]
        public async Task Invoke()
        {
            await authMiddleware.Invoke(httpc.Object);
        }





    }
}
