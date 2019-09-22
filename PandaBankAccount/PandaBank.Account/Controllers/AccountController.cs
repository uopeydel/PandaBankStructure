using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Service.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Contract.Account;
using PandaBank.SharedService.Const;
using PandaBank.Account.Service.Facade.Interface;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Contract.Account.Read;

namespace PandaBank.Account.Controllers
{
    [Authorize]
    [Route("api/Account")]
    public class AccountController : BaseController
    {
        private readonly IAccountFacade _accountFacade;
        public AccountController(IAccountFacade accountFacade)
        {
            _accountFacade = accountFacade;
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
                        Structure = new PandaAccountCreateContract { }
                    }
                 );
            }
            Results<bool> result = await _accountFacade.CreateAccount(IdentityUser, account);
            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }


        [HttpDelete("{AccountId}")]
        public async Task<IActionResult> DeleteAccount([FromRoute]string AccountId)
        {
            Results<bool> result = await _accountFacade.DeleteAccount(IdentityUser, AccountId);
            if (result.IsError())
            {
                return StatusCode(500, result);
            }
            return Ok(result);
        }

        [HttpPost("Deposit")]
        public async Task<IActionResult> Deposit([FromBody] PandaStatementCreateContract Statement)
        {
            Results<bool> result = await _accountFacade.UpdateStatement(IdentityUser, Statement, Enums.PandaStatementStatus.Deposit);
            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpPost("Witdraw")]
        public async Task<IActionResult> Witdraw([FromBody] PandaStatementCreateContract Statement)
        {
            Results<bool> result = await _accountFacade.UpdateStatement(IdentityUser, Statement, Enums.PandaStatementStatus.Witdraw);
            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpGet("All")]
        public async Task<IActionResult> GetAllAccount([FromQuery] PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> result = await _accountFacade.GetAllAccount(paging);

            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }


        [HttpGet("Me")]
        public async Task<IActionResult> GetMyAccount([FromQuery] PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> result = await _accountFacade.GetMyAccount(IdentityUser, paging);

            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

        [HttpGet("Me/{accountId}/Statement")]
        public async Task<IActionResult> GetMyAccountStatement([FromRoute]string accountId, [FromQuery] PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> result = await _accountFacade.GetMyAccountStatement(IdentityUser, accountId, paging);

            if (result.IsError())
            {
                return StatusCode(500, result);
            }

            return Ok(result);
        }

    }
}
