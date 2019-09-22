using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Contract.Account;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Extensions;

namespace PandaBank.Account.Service.Facade.Interface
{
    public interface IAccountFacade
    {
        Task<Results<bool>> CreateAccount(long identityUser, PandaAccountCreateContract account);
        Task<Results<bool>> DeleteAccount(long identityUser, string accountId);
         Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging); 
        Task<Results<bool>> UpdateStatement(long identityUser, PandaStatementCreateContract statement, Enums.PandaStatementStatus status);
    }
}
