using Microsoft.EntityFrameworkCore;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.User.DAL.DataAccess;
using PandaBank.User.DAL;
using PandaBank.User.DAL.Models;
using PandaBank.User.DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.DAL.Repository.Implement
{
    public class UserRepository : IUserRepository
    {
        private readonly IGenericEFRepository<PandaUserDbContext> _repo;
        public UserRepository(IGenericEFRepository<PandaUserDbContext> repo)
        {
            _repo = repo;
        }

        public async Task<bool> ClearRefreshToken(long identityUser)
        {
            var user = await _repo.GetQueryAble<PandaUser>().Where(w => w.Id == identityUser).FirstOrDefaultAsync();
            user.RefreshToken = null;
              _repo.UpdateSpecficProperty(user,u => u.RefreshToken);
            var result = await _repo.SaveAsync();
            return result.Data;
        }

        public async Task<bool> EmailValid(string email)
        {
            return await _repo.GetQueryAble<PandaUser>().Where(w => w.Email.ToLower().Equals(email.ToLower())).AnyAsync();
        }

        public async Task<Results<List<PandaUserSearchResultContract>>> GetAllUser(PagingParameters paging)
        {
            Expression<Func<PandaUser, bool>> predicate = p => true;
            Expression<Func<PandaUser, PandaUserSearchResultContract>> selector = s =>
            new PandaUserSearchResultContract
            {
                Id = s.Id,
                Email = s.Email,
                FirstName = s.FirstName,
                LastName = s.LastName,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt,

            };
            var response = await _repo.GetPagingAsync<PandaUser, PandaUserSearchResultContract>(predicate, selector, paging);
            return response;
        }

        public async Task<string> GetRefreshToken(string email)
        {
            return await _repo.GetQueryAble<PandaUser>().Where(w => w.Email.ToLower().Equals(email.ToLower()))
                .Select(s => s.RefreshToken)
                .FirstOrDefaultAsync();
        }

        public async Task<Results<bool>> UpdateRefreshToken(string email, string refreshToken)
        {
            var pandaUser = await _repo.GetQueryAble<PandaUser>()
                .Where(w => w.Email.ToLower().Equals(email.ToLower()))
                .FirstOrDefaultAsync();
            if (pandaUser != null)
            {
                pandaUser.RefreshToken = refreshToken;
                await _repo.SaveAsync();
                return PandaResponse.CreateSuccessResponse<bool>(true);
            }

            return PandaResponse.CreateErrorResponse<bool>("Error when refresh token");
        }
    }
}
