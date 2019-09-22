using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PandaBank.SharedService.Service.Implement;
using PandaBank.SharedService.Service.Interface;
using PandaBank.Account.DAL.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Validate.ControllerValidate;
using PandaBank.Account.DAL.Repository.Implement;
using PandaBank.Account.DAL.Repository.Interface;
using PandaBank.Account.Service.Facade.Interface;
using PandaBank.Account.Service.Logic.Interface;
using PandaBank.Account.Service.Logic.Implement;
using PandaBank.Account.Service.Facade.Implement;

namespace PandaBank.Account.Service
{
    public static class ServiceExtensions
    {
        public static void AddIoc(this IServiceCollection services)
        {
            #region Transient
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            //services.AddTransient<IEmailSender, EmailSender>();

            //services.AddTransient<IValidator<PandaAccountContract>, CreatePandaAccountValidator>();

            #endregion


            #region Scoped

            services.AddScoped(typeof(IGenericEFRepository<>), typeof(GenericEFRepository<>));
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IAccountFacade, AccountFacade>();
            services.AddScoped<IAccountLogic, AccountLogic>();




            services.AddScoped(typeof(SignInManager<>));
            services.AddScoped(typeof(UserManager<>));

            #endregion


            #region Singleton
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSingleton<UrlRedirectRule>();

            #endregion
        }

    }
}
