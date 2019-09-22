using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.DAL.Repository.Interface
{
    public interface IUserRepository
    {
        Task<Results<bool>> UpdateRefreshToken(string email, string refreshToken);
        Task<string> GetRefreshToken(string email);
        Task<Results<List<PandaUserSearchResultContract>>> GetAllUser(PagingParameters paging);
        Task<bool> EmailValid(string email);
        Task<bool> ClearRefreshToken(long identityUser);
    }
}
