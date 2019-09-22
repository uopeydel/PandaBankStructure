using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Service.Interface;
using PandaBank.Service.Facade.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Contract.Account.Create;

namespace PandaBank.Server.Controllers
{
    [Authorize]
    [Route("api/Account")]
    public class AccountController : BaseController
    { 
        private readonly IGatewayFacade _gatewayFacade;
        public AccountController(IJwtTokenService jwtTokenService, IGatewayFacade gatewayFacade)
        {
            _gatewayFacade = gatewayFacade; 
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateAccount([FromBody]PandaAccountCreateContract account)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new PandaAccountCreateContract
                        {
                            PandaStatement = new List<PandaStatementCreateContract> { new PandaStatementCreateContract {  } },
                            
                        }
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


        [HttpDelete("{AccountId}")]
        public async Task<IActionResult> DeleteAccount([FromRoute]string AccountId)
        { 
            var result = await _gatewayFacade.GateWayInvoke(
             method: BaseMethod,
             token: Token,
             path: BasePath,
             string.Empty,
             query: BaseQueryString
             );
            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] PandaStatementCreateContract Statement)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new PandaStatementCreateContract { }
                    }
                 );
            }
            var result = await _gatewayFacade.GateWayInvoke(
             method: BaseMethod,
             token: Token,
             path: BasePath,
             Statement,
             query: BaseQueryString
             );
            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpPost("Witdraw")]
        public async Task<IActionResult> Witdraw([FromBody] PandaStatementCreateContract Statement)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(500,
                    new
                    {
                        Error = ModelState.Values.SelectMany(s => s.Errors.Select(ss => ss.ErrorMessage)).ToList(),
                        Structure = new PandaStatementCreateContract { }
                    }
                 );
            }
            var result = await _gatewayFacade.GateWayInvoke(
             method: BaseMethod,
             token: Token,
             path: BasePath,
             Statement,
             query: BaseQueryString
             );
            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpGet("All")]
        public async Task<IActionResult> GetAllAccount([FromQuery] PagingParameters paging)
        {
            var result = await _gatewayFacade.GateWayInvoke(
              method: BaseMethod,
              token: Token,
              path: BasePath,
              string.Empty,
              query: BaseQueryString
              );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpGet("Me")]
        public async Task<IActionResult> GetMyAccount([FromQuery] PagingParameters paging)
        {
            var result = await _gatewayFacade.GateWayInvoke(
             method: BaseMethod,
             token: Token,
             path: BasePath,
             string.Empty,
             query: BaseQueryString
             );

            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpGet("Me/{accountId}/Statement")]
        public async Task<IActionResult> GetMyAccountStatement([FromRoute]string accountId, [FromQuery] PagingParameters paging)
        {
            var result = await _gatewayFacade.GateWayInvoke(
             method: BaseMethod,
             token: Token,
             path: BasePath,
             string.Empty,
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
