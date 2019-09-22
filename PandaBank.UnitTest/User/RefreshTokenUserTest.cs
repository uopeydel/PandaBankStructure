using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PandaBank.Account.DAL.DataAccess;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Service.Implement;
using PandaBank.SharedService.Service.Interface;
using PandaBank.User.DAL.Models;
using PandaBank.User.DAL.Repository.Interface;
using PandaBank.User.Service.Facade.Implement;
using PandaBank.User.Service.Logic.Implement;
using PandaBank.User.Service.Logic.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PandaBank.UnitTest.User
{
    public class RefreshTokenUserTest
    {
        private readonly MockRepository mock;
        private readonly Mock<IJwtTokenService> _jwtServiceMocking;
        private readonly Mock<IUserLogic> _userLogicMocking;

        private readonly Mock<IConfiguration> _configMocking;
        private readonly JwtTokenService jwtService;

        private UserFacade userFacade;
        private readonly DateTime now = DateTime.Now;

        public RefreshTokenUserTest()
        {
            mock = new MockRepository(MockBehavior.Default);

            _jwtServiceMocking = new Mock<IJwtTokenService>();
            _userLogicMocking = new Mock<IUserLogic>();

            #region Create JwtService new Instance
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
                .Returns(pandaSecureKey);

            jwtService = new JwtTokenService(_configMocking.Object);

            #endregion




        }


        [Fact]
        public async Task RefreshTokenUserTestSuccess()
        {
            #region Mock Success

            TestRefreshSuccess.prarameter.Token = jwtService.GenerateToken(TestRefreshSuccess.user.Email, TestRefreshSuccess.user.Id.ToString());
            TestRefreshSuccess.prarameter.RefreshToken = jwtService.GenerateRefreshToken();

            var claimPrincipa = jwtService.GetPrincipalFromExpiredToken(TestRefreshSuccess.prarameter.Token);
            _jwtServiceMocking.Setup(s => s.GetPrincipalFromExpiredToken(TestRefreshSuccess.prarameter.Token))
                .Returns(claimPrincipa);


            var emailFromClaim = jwtService.GetValueFromClaimType(claimPrincipa, JwtRegisteredClaimNames.Email);
            _jwtServiceMocking.Setup(s => s.GetValueFromClaimType(claimPrincipa, JwtRegisteredClaimNames.Email))
               .Returns(emailFromClaim);

            var userIdFromClaim = jwtService.GetValueFromClaimType(claimPrincipa, JwtRegisteredClaimNames.NameId);
            _jwtServiceMocking.Setup(s => s.GetValueFromClaimType(claimPrincipa, JwtRegisteredClaimNames.NameId))
              .Returns(userIdFromClaim);


            TestRefreshSuccess.response.Token = jwtService.GenerateToken(claimPrincipa.Claims.ToList());
            TestRefreshSuccess.response.RefreshToken = jwtService.GenerateRefreshToken();

            _jwtServiceMocking.Setup(s => s.GenerateToken(emailFromClaim, userIdFromClaim))
               .Returns(TestRefreshSuccess.response.Token);

            _jwtServiceMocking.Setup(s => s.GenerateRefreshToken())
               .Returns(TestRefreshSuccess.response.RefreshToken);

            _userLogicMocking.Setup(s => s.ValidateRefreshToken(TestRefreshSuccess.user.Email, TestRefreshSuccess.prarameter.RefreshToken))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));

            _userLogicMocking.Setup(s => s.UpdateRefreshTokenToUser(TestRefreshSuccess.user.Email, TestRefreshSuccess.response.RefreshToken))
               .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));
            
            userFacade = new UserFacade(_jwtServiceMocking.Object, _userLogicMocking.Object);

            #endregion

            var result = await userFacade.RefreshToken(TestRefreshSuccess.prarameter);

            _jwtServiceMocking.Verify(v => v.GetPrincipalFromExpiredToken(TestRefreshSuccess.prarameter.Token));
            _jwtServiceMocking.Verify(v => v.GetValueFromClaimType(claimPrincipa, JwtRegisteredClaimNames.NameId));
            _jwtServiceMocking.Verify(v => v.GenerateToken(emailFromClaim, userIdFromClaim));
            _jwtServiceMocking.Verify(v => v.GenerateRefreshToken());

            _userLogicMocking.Verify(v => v.ValidateRefreshToken(TestRefreshSuccess.user.Email, TestRefreshSuccess.prarameter.RefreshToken));

            _userLogicMocking.Verify(v => v.UpdateRefreshTokenToUser(TestRefreshSuccess.user.Email, TestRefreshSuccess.response.RefreshToken));


            Assert.Equal(TestRefreshSuccess.response.RefreshToken, result.Data.RefreshToken);
            Assert.Equal(TestRefreshSuccess.response.Token, result.Data.Token);
        }

    }


    public static class TestRefreshSuccess
    {
        public static PandaUserLoginContract user = new PandaUserLoginContract
        {
            Email = "Success@email.email",
            Id = 1,
        };

        public static RefreshTokenContract prarameter = new RefreshTokenContract
        {
            Token = null,
            RefreshToken = null,
        };

        public static RefreshTokenContract response = new RefreshTokenContract
        {
            Token = null,
            RefreshToken = null,
        };




        public static class TestRefreshFail
        {
            public static PandaUserLoginContract user = new PandaUserLoginContract
            {
                Email = "Fail@email.email",
                Id = 0,
            };

            public static RefreshTokenContract prarameter = new RefreshTokenContract
            {
                Token = null,
                RefreshToken = null,
            };

            public static RefreshTokenContract response = new RefreshTokenContract
            {
                Token = null,
                RefreshToken = null,
            };
        }
    }
}


