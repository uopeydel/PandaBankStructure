using PandaBank.Account.DAL.Models;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Account.DAL.Repository.Interface
{
    public interface IAccountRepository
    {
        Task<bool> GetAccountStatus(string pandaAccountId);
        Task<bool> IsPaticipant(string PandaAccountId ,long identityUser);
        Task<Results<bool>> Create(PandaAccount newAccount);
        Task<Results<bool>> CreateStatement(PandaStatement newStatement);
        Task<Results<bool>> UpdateAccountBalance(string PandaAccountId, double Balances);
        Task<Results<bool>> UnActiveAccount(long identityUser, string accountId);
        Task<double> GetAccountBalance(string pandaAccountId);
        Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging);
        Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging);
    }
}
