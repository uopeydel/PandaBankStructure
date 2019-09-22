using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.Service.Facade.Interface
{
    public interface IUserFacade
    {
        Task<Results<bool>> Logout(long IdentityUser);
        Task<Results<RefreshTokenContract>> Login(PandaUserLoginContract account);
        Task<Results<RefreshTokenContract>> RefreshToken(RefreshTokenContract token);
        Task<Results<PandaUserContract>> GetMyAccount(long IdentityUser);
        Task<Results<List<PandaUserSearchResultContract>>> GetAllAccount(PagingParameters paging);
        Task<Results<long>> CreateUser(PandaUserContract newAccount);
    }
}
