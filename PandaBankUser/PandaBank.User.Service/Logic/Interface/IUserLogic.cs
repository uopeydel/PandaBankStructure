using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.User.DAL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.Service.Logic.Interface
{
    public interface IUserLogic
    {
        Task<Results<PandaUser>> Login(PandaUserLoginContract account);
        Task<Results<bool>> UpdateRefreshTokenToUser(
            string email,
            string refreshToken
            );

        Task<Results<bool>> ValidateRefreshToken(
            string email,
            string refreshToken
            );

        Task<Results<PandaUserContract>> GetMyAccount(long IdentityUser);
        Task<Results<List<PandaUserSearchResultContract>>> GetAllAccount(PagingParameters paging);
        Task<Results<long>> CreateUser(PandaUserContract newAccount);

        Task<bool> RefreshTokenIsNullOrEmpty(string email);
        Task<bool> Logout(long identityUser);
    }
}
