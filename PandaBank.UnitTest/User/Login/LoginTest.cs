﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PandaBank.Account.DAL.DataAccess;
using PandaBank.SharedService.Contract;
using PandaBank.User.DAL.Models;
using PandaBank.User.DAL.Repository.Interface;
using PandaBank.User.Service.Logic.Implement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PandaBank.UnitTest.Login.User
{
    public class LoginTest
    {

        private readonly Mock<IUserRepository> _userRepoMocking;
        private readonly Mock<SignInManager<PandaUser>> _signInManagerMocking;
        private readonly Mock<UserManager<PandaUser>> _userManagerMocking;
        private readonly Mock<IMapper> _mapperMocking;

        private readonly UserLogic userLogic;
        private readonly DateTime now = DateTime.Now;
        public LoginTest()
        {
            var mock = new MockRepository(MockBehavior.Default);
            _userRepoMocking = mock.Create<IUserRepository>();

            _userRepoMocking = new Mock<IUserRepository>();

            _userManagerMocking = new Mock<UserManager<PandaUser>>(
                  new Mock<IUserStore<PandaUser>>().Object,
                  new Mock<IOptions<IdentityOptions>>().Object,
                  new Mock<IPasswordHasher<PandaUser>>().Object,
                  new IUserValidator<PandaUser>[0],
                  new IPasswordValidator<PandaUser>[0],
                  new Mock<ILookupNormalizer>().Object,
                  new Mock<IdentityErrorDescriber>().Object,
                  new Mock<IServiceProvider>().Object,
                  new Mock<ILogger<UserManager<PandaUser>>>().Object
                  );

            _signInManagerMocking = new Mock<SignInManager<PandaUser>>(
                    _userManagerMocking.Object,
                    new Mock<IHttpContextAccessor>().Object,
                    new Mock<IUserClaimsPrincipalFactory<PandaUser>>().Object,
                    new Mock<IOptions<IdentityOptions>>().Object,
                    new Mock<ILogger<SignInManager<PandaUser>>>().Object,
                    new Mock<IAuthenticationSchemeProvider>().Object);


            _mapperMocking = new Mock<IMapper>();

            #region LoginSuccess

            _userRepoMocking
                .Setup(s => s.EmailValid(TestDataPandaLoginTest.pandaUser.Email))
                .Returns(Task.FromResult(true));

            _signInManagerMocking
                .Setup(s => s.PasswordSignInAsync(TestDataPandaLoginTest.pandaUser.Email, TestDataPandaLoginTest.pandaUser.Password, false, false))
                .Returns(Task.FromResult(SignInResult.Success));

            _userManagerMocking
                .Setup(s => s.FindByEmailAsync(TestDataPandaLoginTest.pandaUser.Email))
                .Returns(Task.FromResult(TestDataPandaLoginTest.CreateUserSuccessPandaUser(now)));

            #endregion

            userLogic = new UserLogic(
                _userManagerMocking.Object,
                _signInManagerMocking.Object,
                _userRepoMocking.Object,
                _mapperMocking.Object);
        }


        [Fact]
        public async Task LoginSuccess()
        {
            var result = await userLogic.Login(TestDataPandaLoginTest.pandaUser);
            var newAccount = TestDataPandaLoginTest.CreateUserSuccessPandaUser(now);

            //For Login fail
            _userRepoMocking
                .Verify(umm => umm.EmailValid(TestDataPandaLoginTest.pandaUser.Email), Times.Never());

            _signInManagerMocking.Verify(umm => umm.PasswordSignInAsync(TestDataPandaLoginTest.pandaUser.Email, TestDataPandaLoginTest.pandaUser.Password, false, false));


            Assert.Equal(result.Data.Email, newAccount.Email);
            Assert.Equal(result.Data.FirstName, newAccount.FirstName);
            Assert.Equal(result.Data.LastName, newAccount.LastName);

            Assert.False(result.IsError());
        }


    }



    public static class TestDataPandaLoginTest
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
            };

            return newAccount;
        }
    }



}
