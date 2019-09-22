using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PandaBank.Service.Facade.Interface;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Service.Interface;
using PandaBank.SharedService.Validate;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace PandaBank.Server.Controllers
{

    [Authorize]
    [Route("api/User")]
    public class UserController : BaseController
    {
        private readonly IGatewayFacade _gatewayFacade;
        public UserController(IGatewayFacade gatewayFacade)
        {
            _gatewayFacade = gatewayFacade;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> CreateUser([FromBody]PandaUserContract newAccount)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new PandaUserContract { }
                    }
                 );
            }

            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                newAccount,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);

        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<IActionResult> Login([FromBody]PandaUserLoginContract account)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new PandaUserLoginContract { }
                    }
                 );
            }

            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                account,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenContract token)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new RefreshTokenContract { }
                    }
                 );
            }

            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                token,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [HttpPost("me")]
        public async Task<IActionResult> Me()
        {
            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                body: string.Empty,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                body: string.Empty,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        //TODO:Permission
        [HttpPost("all")]
        public async Task<IActionResult> GetAll([FromQuery] PagingParameters paging)
        {
            var result = await _gatewayFacade.GateWayInvoke(
                method: BaseMethod,
                token: Token,
                path: BasePath,
                body: string.Empty,
                query: BaseQueryString
                );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

    }
}
