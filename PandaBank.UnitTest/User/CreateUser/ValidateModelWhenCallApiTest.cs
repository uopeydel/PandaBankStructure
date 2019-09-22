using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Extensions;
using PandaBank.User;
using PandaBank.User.Service.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using Newtonsoft.Json;
using FluentValidation;
using PandaBank.SharedService.Validate.ControllerValidate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using PandaBank.SharedService.Validate.ControllerValidate.User;

namespace PandaBank.UnitTest.User.CreateUser
{
    public class ValidateModelWhenCallApiTest
        : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly WebApplicationFactory<Startup> _factory;

        private Mock<IUserFacade> userFacadeMocking;

        private HttpClient client;
        public ValidateModelWhenCallApiTest(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;

            var mock = new MockRepository(MockBehavior.Default);
            userFacadeMocking = mock.Create<IUserFacade>();
            userFacadeMocking.Setup(set => set.CreateUser(It.IsAny<PandaUserContract>()))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse((long)1)));

            void ConfigureTestServices(IServiceCollection services)
            {
                services.AddSingleton<IValidator<PandaUserContract>, CreatePandaUserValidator>();
                services.AddSingleton<IUserFacade>(userFacadeMocking.Object);
            };
            client = _factory
                .WithWebHostBuilder(builder => builder.ConfigureTestServices(ConfigureTestServices))
                .CreateClient();
             
        }

        [Fact]
        public async Task CreateError()
        {
            var jsonData = new PandaUserContract
            {
                Email = "",
                Password = "",
                LastName = "",
                FirstName = "",
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
            content.Headers.Add("Panda", "PandaBankUserSecureKey");
            var profile = await client.PostAsync("/api/User", content);
            var result = await profile.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.InternalServerError, profile.StatusCode);
        }


        [Fact]
        public async Task CreateSuccess()
        {
            var jsonData = new PandaUserContract
            {
                Email = "test@test.test",
                Password = "123456",
                LastName = "test123456",
                FirstName = "test123456",
            };

            HttpContent content = new StringContent(JsonConvert.SerializeObject(jsonData), Encoding.UTF8, "application/json");
            content.Headers.Add("Panda", "PandaBankUserSecureKey");
            var profile = await client.PostAsync("/api/User", content);
            var result = await profile.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.OK, profile.StatusCode);
        }

    }
}
