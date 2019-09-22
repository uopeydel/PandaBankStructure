using PandaBank.Account.Service.Facade.Interface;
using PandaBank.Account.Service.Logic.Interface;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Contract.Account;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Account.Service.Facade.Implement
{
    public class AccountFacade : IAccountFacade
    {
        private readonly IAccountLogic _accountLogic;
        public AccountFacade(IAccountLogic accountLogic)
        {
            _accountLogic = accountLogic;
        }

        public async Task<Results<bool>> CreateAccount(long identityUser, PandaAccountCreateContract account)
        {
            account.Balances = 0;
            account.Active = true;
            account.UserAccounts = new List<UserAccountCreateContract> { new UserAccountCreateContract { PandaUserId = identityUser } };
            account.Id = await _accountLogic.GenerateAccountId();
            
            if (string.IsNullOrEmpty(account.Id))
            {
                return PandaResponse.CreateErrorResponse<bool>("Error while generate account id");
            }
            
            Results<bool> createAccountResult = await _accountLogic.CreateAccount(account);
            if (string.IsNullOrEmpty(account.Id))
            {
                createAccountResult.Errors.Insert(0, "Error while create account");
                return createAccountResult;
            }

            //_accountLogic.AddPaticipant

            account.PandaStatement.FirstOrDefault().PandaAccountId = account.Id;
            var depositInitResult = await UpdateStatement(identityUser, account.PandaStatement.FirstOrDefault(), Enums.PandaStatementStatus.Deposit);
            if (depositInitResult.IsError())
            {
                depositInitResult.Errors.Insert(0, "Error while deposit money");
                return depositInitResult;
            }

            return PandaResponse.CreateSuccessResponse(true);
        }

        public async Task<Results<bool>> DeleteAccount(long identityUser, string accountId)
        {
            Results<bool> unActiveResult = await _accountLogic.UnActiveAccount(identityUser, accountId);
            if (unActiveResult.IsError())
            {
                return unActiveResult;
            }
            bool canActive = await _accountLogic.CanActiveAccount(identityUser, accountId);
            if (canActive != false)
            {
                return PandaResponse.CreateErrorResponse<bool>("Something wrong with account");
            }
            return PandaResponse.CreateSuccessResponse(canActive);

        }

        public async Task<Results<bool>> UpdateStatement(long identityUser, PandaStatementCreateContract statement, Enums.PandaStatementStatus status)
        {
            statement.Status = status;
            bool canActive = await _accountLogic.CanActiveAccount(identityUser, statement.PandaAccountId);
            if (!canActive)
            {
                return PandaResponse.CreateErrorResponse<bool>("Account suspended");
            }

            bool canUpdateBalance = await _accountLogic.CanUpdateBalance(statement.PandaAccountId, statement.Balances, status);
            if (canUpdateBalance == false)
            {
                return PandaResponse.CreateErrorResponse<bool>("Can not update balance");
            }
            var runStatementResult = await _accountLogic.RunStatement(statement);
            return runStatementResult;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> results = await _accountLogic.GetAllAccount(paging);
            return results;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> results = await _accountLogic.GetMyAccount(identityUser, paging);
            return results;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> results = await _accountLogic.GetMyAccountStatement(identityUser, accountId, paging);
            return results;
        }


    }
}
