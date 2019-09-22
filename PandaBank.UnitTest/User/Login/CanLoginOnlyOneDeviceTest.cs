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
using PandaBank.User.DAL.Models;
using PandaBank.User.DAL.Repository.Interface;
using PandaBank.User.Service.Facade.Implement;
using PandaBank.User.Service.Logic.Implement;
using PandaBank.User.Service.Logic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PandaBank.UnitTest.User.Login
{
    public class CanLoginOnlyOneDeviceTest
    {
        private readonly MockRepository mock;
        private readonly Mock<IConfiguration> _configMocking;
        private readonly JwtTokenService jwtService;
        private readonly Mock<IUserLogic> _userLogicMocking;


        private readonly UserFacade userFacade;
        private readonly DateTime now = DateTime.Now;

        public CanLoginOnlyOneDeviceTest()
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
                .Returns(pandaSecureKey);

            jwtService = new JwtTokenService(_configMocking.Object);
            _userLogicMocking = new Mock<IUserLogic>();

            #region Success

            _userLogicMocking.Setup(s => s.Logout(1))
               .Returns(Task.FromResult(true));

            _userLogicMocking.Setup(s => s.RefreshTokenIsNullOrEmpty(TestDataPandaLoginOneDeviceSuccess.pandaUser.Email))
                .Returns(Task.FromResult(true));

            _userLogicMocking.Setup(s => s.Login(TestDataPandaLoginOneDeviceSuccess.pandaUser))
              .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(TestDataPandaLoginOneDeviceSuccess.CreateUserSuccessPandaUser(now))));

            _userLogicMocking.Setup(s => s.UpdateRefreshTokenToUser(TestDataPandaLoginOneDeviceSuccess.pandaUser.Email, It.IsAny<string>()))
              .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));


            #endregion

            #region Fail


            _userLogicMocking.Setup(s => s.Logout(0))
                .Returns(Task.FromResult(false));

            #endregion


            userFacade = new UserFacade(
                jwtService,
                _userLogicMocking.Object);
        }


        [Fact]
        public async Task CanLoginOnlyOneDevice()
        {
            var result = await userFacade.Login(TestDataPandaLoginOneDeviceSuccess.pandaUser);
            _userLogicMocking.Verify(v => v.Login(TestDataPandaLoginOneDeviceSuccess.pandaUser));
            _userLogicMocking.Verify(v => v.UpdateRefreshTokenToUser(It.IsAny<string>(), It.IsAny<string>()));
            Assert.NotNull(result.Data.RefreshToken);
            Assert.NotEmpty(result.Data.RefreshToken);
            Assert.NotNull(result.Data.Token);
            Assert.NotEmpty(result.Data.Token);

            var resultFail = await userFacade.Login(TestDataPandaLoginOneDeviceFail.pandaUser);
            _userLogicMocking.Verify(v => v.RefreshTokenIsNullOrEmpty(TestDataPandaLoginOneDeviceFail.pandaUser.Email));
            var failUser = TestDataPandaLoginOneDeviceFail.CreateUserFailPandaUser(now);
            Assert.False(string.IsNullOrEmpty(failUser.RefreshToken));
            Assert.True(resultFail.IsError());
            _userLogicMocking.Verify(v => v.Login(TestDataPandaLoginOneDeviceFail.pandaUser), Times.Never());
        }


        [Fact]
        public async Task LogoutSuccess()
        {
            var result = await userFacade.Logout(1);
            Assert.True(result.Data);
        }

        [Fact]
        public async Task LogoutFail()
        {
            var result = await userFacade.Logout(0);
            Assert.False(result.Data);
        }

    }


    public static class TestDataPandaLoginOneDeviceSuccess
    {
        public static PandaUserContract pandaUser = new PandaUserContract
        {
            Email = "test@test.test",
            FirstName = "testname",
            LastName = "testlastname",
            Password = "testpassword"
        };

        public static PandaUser CreateUserSuccessPandaUser(DateTime dateTime)
        {
            PandaUser newAccount = new PandaUser
            {
                Email = pandaUser.Email,
                FirstName = pandaUser.FirstName,
                LastName = pandaUser.LastName,
                UserName = pandaUser.Email,
                CreatedAt = dateTime,
                UpdatedAt = dateTime,
                RefreshToken = null,
            };

            return newAccount;
        }
    }



    public static class TestDataPandaLoginOneDeviceFail
    {
        public static PandaUserContract pandaUser = new PandaUserContract
        {
            Email = "test@fail.fail",
            FirstName = "failname",
            LastName = "faillastname",
            Password = "failpassword"
        };

        public static PandaUser CreateUserFailPandaUser(DateTime dateTime)
        {
            PandaUser newAccount = new PandaUser
            {
                Email = pandaUser.Email,
                FirstName = pandaUser.FirstName,
                LastName = pandaUser.LastName,
                UserName = pandaUser.Email,
                CreatedAt = dateTime,
                UpdatedAt = dateTime,
                RefreshToken = "refresh token is have",
            };

            return newAccount;
        }
    }
}
