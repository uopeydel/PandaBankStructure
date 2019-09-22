using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PandaBank.SharedService.Service.Implement;
using PandaBank.SharedService.Service.Interface;
using PandaBank.Service.Facade.Implement;
using PandaBank.Service.Facade.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PandaBank.Service.Logic.Implement;
using PandaBank.Service.Logic.Interface;
using FluentValidation;
using PandaBank.SharedService.Validate.ControllerValidate;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Validate.ControllerValidate.User;
using PandaBank.SharedService.Validate.ControllerValidate.Account;
using PandaBank.SharedService.Contract.Account.Create;

namespace PandaBank.Server.Service
{
    public static class ServiceExtensions
    {
        public static void AddIoc(this IServiceCollection services)
        {
            #region Transient
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            //services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IValidator<PandaUserLoginContract>, LoginPandaUserValidator>();
            services.AddTransient<IValidator<PandaUserContract>, CreatePandaUserValidator>();
            services.AddTransient<IValidator<RefreshTokenContract>, RefreshTokenValidator>();

            services.AddTransient<IValidator<PandaAccountCreateContract>, CreateAccountValidator>();
            services.AddTransient<IValidator<PandaStatementCreateContract>, StatementValidator>();

            #endregion


            #region Scoped

            //services.AddScoped(typeof(IGenericEFRepository<>), typeof(GenericEFRepository<>));

            services.AddScoped<IGatewayFacade, GatewayFacade>();
            services.AddScoped<IGatewayLogic, GatewayLogic>();



            services.AddScoped(typeof(SignInManager<>));
            services.AddScoped(typeof(UserManager<>));

            #endregion


            #region Singleton
            //var mappingConfig = new MapperConfiguration(mc =>
            //{
            //    mc.AddProfile(new MappingProfile());
            //});
            //IMapper mapper = mappingConfig.CreateMapper();
            //services.AddSingleton(mapper);


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //services.AddSingleton<UrlRedirectRule>();

            #endregion
        }

    }
}
