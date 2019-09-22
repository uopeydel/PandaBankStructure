using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.UnitTest.User
{
    public class FakeUserManager : UserManager<PandaUser>
    {
        public FakeUserManager()
            : base(new Mock<IUserStore<PandaUser>>().Object,
              new Mock<IOptions<IdentityOptions>>().Object,
              new Mock<IPasswordHasher<PandaUser>>().Object,
              new IUserValidator<PandaUser>[0],
              new IPasswordValidator<PandaUser>[0],
              new Mock<ILookupNormalizer>().Object,
              new Mock<IdentityErrorDescriber>().Object,
              new Mock<IServiceProvider>().Object,
              new Mock<ILogger<UserManager<PandaUser>>>().Object)
        {

        }

        public override Task<IdentityResult> CreateAsync(PandaUser user, string password)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<IdentityResult> AddToRoleAsync(PandaUser user, string role)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        public override Task<string> GenerateEmailConfirmationTokenAsync(PandaUser user)
        {
            return Task.FromResult(Guid.NewGuid().ToString());
        }

    }

    public class FakeSignInManager : SignInManager<PandaUser>
    {
        public FakeSignInManager()
                : base(new FakeUserManager(),
                     new Mock<IHttpContextAccessor>().Object,
                     new Mock<IUserClaimsPrincipalFactory<PandaUser>>().Object,
                     new Mock<IOptions<IdentityOptions>>().Object,
                     new Mock<ILogger<SignInManager<PandaUser>>>().Object,
                     new Mock<IAuthenticationSchemeProvider>().Object)
        {

        }
    }

}
