using AutoMapper;
using PandaBank.Account.DAL.Models;
using PandaBank.Account.DAL.Repository.Interface;
using PandaBank.Account.Service.Logic.Interface;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Account.Service.Logic.Implement
{
    public class AccountLogic : IAccountLogic
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        public AccountLogic(IAccountRepository accountRepository, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
        }


        public async Task<bool> CanActiveAccount(long identityUser, string PandaAccountId)
        {
            bool acountActive = await _accountRepository.GetAccountStatus(PandaAccountId);
            bool isPaticipant = await _accountRepository.IsPaticipant(PandaAccountId, identityUser);
            return acountActive && isPaticipant;
        }

        public async Task<bool> CanUpdateBalance(string pandaAccountId, double balancesForUpdate, Enums.PandaStatementStatus status)
        {
            var notAllowBalanceUpdateEqual0 = balancesForUpdate == 0;
            if (notAllowBalanceUpdateEqual0)
            {
                return false;
            }

            var depositMustMoreThan0 =
                status == Enums.PandaStatementStatus.Deposit && balancesForUpdate > 0;
            if (depositMustMoreThan0)
            {
                return true;
            }

            var depositCantLessThan0
                = status == Enums.PandaStatementStatus.Deposit && balancesForUpdate < 0;
            if (depositCantLessThan0)
            {
                return false;
            }

            var WitdrawMustMoreThan0 =
                status == Enums.PandaStatementStatus.Witdraw && balancesForUpdate > 0;
            if (WitdrawMustMoreThan0)
            {
                return false;
            }

            double accountBalance = await _accountRepository.GetAccountBalance(pandaAccountId);
            return ((balancesForUpdate + accountBalance) >= 0);
        }

        public async Task<Results<bool>> CreateAccount(PandaAccountCreateContract account)
        {
            var newAccount = new PandaAccount();
            _mapper.Map(account, newAccount);
            var createResult = await _accountRepository.Create(newAccount);
            return createResult;
        }

        public async Task<string> GenerateAccountId()
        {
            //TODO : find the best way to generate bank account!!

            var guidGenerate = await Task.FromResult(Guid.NewGuid().ToString());

            return guidGenerate;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> accountResult = await _accountRepository.GetAllAccount(paging);
            return accountResult;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> accountResult = await _accountRepository.GetMyAccount(identityUser, paging);
            return accountResult;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging)
        {
            Results<List<PandaAccountReadContract>> accountResult = await _accountRepository.GetMyAccountStatement(identityUser, accountId, paging);
            return accountResult;
        }

        public async Task<Results<bool>> RunStatement(PandaStatementCreateContract statement)
        {
            if (string.IsNullOrEmpty(statement.PandaAccountId))
            {
                return PandaResponse.CreateErrorResponse<bool>("Account id not found");
            }
            statement.CreatedAt = statement.CreatedAt == null ? DateTime.Now : statement.CreatedAt;
            var newStatement = new PandaStatement();
            _mapper.Map(statement, newStatement);
            Results<bool> runStateMentResult = await _accountRepository.CreateStatement(newStatement);
            if (runStateMentResult.IsError())
            {
                return runStateMentResult;
            }
            if (string.IsNullOrEmpty(newStatement.PandaAccountId))
            {
                return PandaResponse.CreateErrorResponse<bool>("Panda account Id Invalid");
            }
            Results<bool> balanceIsUpdated = await _accountRepository.UpdateAccountBalance(newStatement.PandaAccountId, newStatement.Balances);
            if (balanceIsUpdated.IsError())
            {
                return balanceIsUpdated;
            }
            return PandaResponse.CreateSuccessResponse(true);
        }

        public async Task<Results<bool>> UnActiveAccount(long identityUser, string accountId)
        {
            Results<bool> result = await _accountRepository.UnActiveAccount(identityUser, accountId);
            return result;
        }
    }
}
