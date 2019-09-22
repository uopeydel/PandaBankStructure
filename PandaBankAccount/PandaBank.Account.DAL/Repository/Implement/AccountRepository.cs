using Microsoft.EntityFrameworkCore;
using PandaBank.Account.DAL.DataAccess;
using PandaBank.Account.DAL.Models;
using PandaBank.Account.DAL.Repository.Interface;
using PandaBank.SharedService.Contract.Account.Read;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Account.DAL.Repository.Implement
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IGenericEFRepository<PandaAccountDbContext> _repo;
        public AccountRepository(IGenericEFRepository<PandaAccountDbContext> repo)
        {
            _repo = repo;
        }

        public async Task<Results<bool>> Create(PandaAccount newAccount)
        {
            await _repo.AddAsync(newAccount);
            var result = await _repo.SaveAsync();
            return result;
        }

        public async Task<Results<bool>> CreateStatement(PandaStatement newStatement)
        {
            await _repo.AddAsync(newStatement);
            var result = await _repo.SaveAsync();
            return result;
        }

        public async Task<double> GetAccountBalance(string pandaAccountId)
        {
            return await _repo.GetQueryAble<PandaAccount>()
                .Where(w => w.Id == pandaAccountId)
                .Select(s => s.Balances).FirstOrDefaultAsync();
        }

        public async Task<bool> GetAccountStatus(string pandaAccountId)
        {
            return await _repo.GetQueryAble<PandaAccount>()
                .Where(w => w.Id == pandaAccountId)
                .Select(s => s.Active).FirstOrDefaultAsync();
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetAllAccount(PagingParameters paging)
        {
            Expression<Func<PandaAccount, bool>> predicate = p => p.Active;
            Expression<Func<PandaAccount, PandaAccountReadContract>> selector = s =>
            new PandaAccountReadContract
            {
                Id = s.Id,
                Active = s.Active,
                Balances = s.Balances,
                Description = s.Description,
                Name = s.Name,
                UserAccounts = s.UserAccounts.Select(ss => new UserAccountReadContract
                {
                    PandaAccountId = ss.PandaAccountId,
                    PandaUserId = ss.PandaUserId
                }).ToList()

            };
            var response = await _repo.GetPagingAsync<PandaAccount, PandaAccountReadContract>(predicate, selector, paging);
            return response;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccount(long identityUser, PagingParameters paging)
        {
            Expression<Func<PandaAccount, bool>> predicate = p => p.UserAccounts.Any(a => a.PandaUserId == identityUser) && p.Active;
            Expression<Func<PandaAccount, PandaAccountReadContract>> selector = s =>
            new PandaAccountReadContract
            {
                Id = s.Id,
                Active = s.Active,
                Balances = s.Balances,
                Description = s.Description,
                Name = s.Name,
                UserAccounts = s.UserAccounts.Select(ss => new UserAccountReadContract
                {
                    PandaAccountId = ss.PandaAccountId,
                    PandaUserId = ss.PandaUserId
                }).ToList()

            };
            var response = await _repo.GetPagingAsync<PandaAccount, PandaAccountReadContract>(predicate, selector, paging);
            return response;
        }

        public async Task<Results<List<PandaAccountReadContract>>> GetMyAccountStatement(long identityUser, string accountId, PagingParameters paging)
        {
            Expression<Func<PandaAccount, bool>> predicate = p =>
            p.Active
            && p.UserAccounts.Any(a => a.PandaUserId == identityUser)
            && p.Id == accountId;
            Expression<Func<PandaAccount, PandaAccountReadContract>> selector = s =>
            new PandaAccountReadContract
            {
                Id = s.Id,
                Active = s.Active,
                Balances = s.Balances,
                Description = s.Description,
                Name = s.Name,
                PandaStatement = s.PandaStatement.Select(ss => new PandaStatementReadContract
                {
                    Balances = ss.Balances,
                    CreatedAt = ss.CreatedAt,
                    PandaAccountId = ss.PandaAccountId,
                    Status = ss.Status
                }).ToList()
            };

            var response = await _repo.GetPagingAsync<PandaAccount, PandaAccountReadContract>(predicate, selector, paging);
            return response;
        }

        public async Task<bool> IsPaticipant(string PandaAccountId, long identityUser)
        {
            return await _repo.GetQueryAble<UserAccount>()
               .Where(w => w.PandaUserId == identityUser && w.PandaAccountId == PandaAccountId && w.PandaAccount.Active)
               .AnyAsync();
        }

        public async Task<Results<bool>> UnActiveAccount(long identityUser, string accountId)
        {
            var account = await _repo.GetQueryAble<PandaAccount>()
                 .Where(w => w.Id == accountId && w.UserAccounts.Any(a => a.PandaUserId == identityUser))
                 .FirstOrDefaultAsync();
            account.Active = false;
            _repo.UpdateSpecficProperty(account, aa => aa.Active);
            return await _repo.SaveAsync();
        }

        public async Task<Results<bool>> UpdateAccountBalance(string PandaAccountId, double Balances)
        {
            var account = await _repo.GetQueryAble<PandaAccount>()
                .Where(w => w.Id == PandaAccountId)
                .FirstOrDefaultAsync();
            account.Balances += Balances;
            _repo.UpdateSpecficProperty(account, aa => aa.Balances);
            return await _repo.SaveAsync();
        }

    }
}
