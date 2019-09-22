using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PandaBank.SharedService.Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static PandaBank.SharedService.Const.Enums;

namespace PandaBank.SharedService.Middleware
{
    public class AuthenticationMiddlewareCustom
    {
        private readonly RequestDelegate _next;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        // Dependency Injection
        public AuthenticationMiddlewareCustom(
            RequestDelegate next,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration
            )
        {
            _next = next;
            _jwtTokenService = jwtTokenService;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            //var apiRoute = context.Request.Path.Value;
            //Enum.TryParse(context.Request.Method, out HttpMethod httpMethod);
            //Read the Header

            var request = context.Request;

            string authHeader = context.Request.Headers["Authorization"];
            string pandaHeader = context.Request.Headers["Panda"];
            if (string.IsNullOrEmpty(pandaHeader))
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("{ errors : ['Secure Key Not found'] }");
                return;
            }

            var SecureKey = _configuration["SecureKey:Self"];
            if (SecureKey != pandaHeader)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("{ errors : ['Secure Key Invalid'] }");
                return;
            }


            ClaimsPrincipal pcp = null;
            if (!string.IsNullOrEmpty(authHeader))
            {
                authHeader = authHeader.Replace("Bearer ", "");
                pcp = _jwtTokenService.GetPrincipalFromExpiredToken(authHeader);
                if (pcp == null)
                {
                    context.Response.StatusCode = 500;
                    await context.Response.WriteAsync("{ errors : ['Token Invalid'] }");
                    return;
                }
            }

            #region BodyModified

            if (request.Path.Value.Contains("/api") && !request.Path.Value.ToLower().Contains("value"))
            {
                var stream = request.Body;
                var originalContent = new StreamReader(stream).ReadToEnd();
                var notModified = true;
                try
                {
                    dynamic dataSource = JsonConvert.DeserializeObject(originalContent);
                    if (dataSource != null)
                    {
                        dataSource.Token = authHeader;
                        var json = JsonConvert.SerializeObject(dataSource);
                        var requestContent = new System.Net.Http.StringContent(json, Encoding.UTF8, "application/json");
                        stream = await requestContent.ReadAsStreamAsync();
                        notModified = false;
                    }
                }
                catch
                {
                    notModified = true;
                }
                if (notModified)
                {
                    var requestData = Encoding.UTF8.GetBytes(originalContent);
                    stream = new MemoryStream(requestData);
                }

                request.Body = stream;
            }
            #endregion


            await _next(context);
        }


    }
}
