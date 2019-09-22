using AutoMapper;
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

namespace PandaBank.UnitTest.CreateUser.User
{
    public class CreateUserTest
    {
        private readonly Mock<IUserRepository> _userRepoMocking;
        private readonly Mock<SignInManager<PandaUser>> _signInManagerMocking;
        private readonly Mock<UserManager<PandaUser>> _userManagerMocking;
        private readonly Mock<IMapper> _mapperMocking;

        private readonly UserLogic userLogic;
        public CreateUserTest()
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

            PandaUser nullUser = null;
            _userManagerMocking
                .Setup(s => s.FindByEmailAsync(TestDataCreateUserTest.pandaUser.Email))
                .Returns(Task.FromResult(nullUser));


          
            _userManagerMocking
                .Setup(s => s.CreateAsync(It.IsAny<PandaUser>(), TestDataCreateUserTest.pandaUser.Password))
                .Returns(Task.FromResult(IdentityResult.Success));

            userLogic = new UserLogic(
                _userManagerMocking.Object,
                _signInManagerMocking.Object,
                _userRepoMocking.Object,
                _mapperMocking.Object); 
        }


        [Fact]
        public async Task CreateUserSuccess()
        {
            var result = await userLogic.CreateUser(TestDataCreateUserTest.pandaUser);
            var newAccount = TestDataCreateUserTest.CreateUserSuccessPandaUser(userLogic.now);
            _userManagerMocking.Verify(umm => umm.FindByEmailAsync(TestDataCreateUserTest.pandaUser.Email));
            _userManagerMocking.Verify(umm => umm.CreateAsync(It.IsAny<PandaUser>(), TestDataCreateUserTest.pandaUser.Password));
            Assert.False(result.IsError());
        }

    }

    public static class TestDataCreateUserTest
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
