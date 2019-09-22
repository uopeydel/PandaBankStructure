using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Extensions;

namespace PandaBank.Account.Service.Logic.Interface
{
    public interface IAccountLogic
    {
        Task<string> GenerateAccountId();
        Task<Results<bool>> CreateAccount(PandaAccountCreateContract account);
        Task<Results<bool>> RunStatement(PandaStatementCreateContract statement);
        Task<bool> CanActiveAccount(long identityUser, string PandaAccountId);
        Task<Results<bool>> UnActiveAccount(long identityUser, string accountId);
        Task<bool> CanUpdateBalance(string pandaAccountId, double balancesForUpdate, Enums.PandaStatementStatus status);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging);
    }
}
