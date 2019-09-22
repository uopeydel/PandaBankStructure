using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Service.Interface;
using PandaBank.User.DAL.Models;
using PandaBank.User.Service.Facade.Interface;
using PandaBank.User.Service.Logic.Interface;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.Service.Facade.Implement
{
    public class UserFacade : IUserFacade
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUserLogic _userLogic;
        public UserFacade(
            IJwtTokenService jwtTokenService,
            IUserLogic userLogic)
        {
            _jwtTokenService = jwtTokenService;
            _userLogic = userLogic;
        }


        public async Task<Results<RefreshTokenContract>> Login(PandaUserLoginContract account)
        {
            var refreshTokenIsNullOrEmpty = await _userLogic.RefreshTokenIsNullOrEmpty(account.Email);
            if (!refreshTokenIsNullOrEmpty)
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>("User still loged in.");
            }

            var signInResult = await _userLogic.Login(account);
            if (signInResult.IsError())
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>(signInResult.Errors.ToArray());
            }
            account.Id = signInResult.Data.Id;

            var token = _jwtTokenService.GenerateToken(account.Email, account.Id.ToString());
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var tokenData = new RefreshTokenContract { RefreshToken = refreshToken, Token = token };

            var refreshTokenResult = await _userLogic.UpdateRefreshTokenToUser(account.Email, refreshToken);
            if (refreshTokenResult.IsError())
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>(refreshTokenResult.Errors.ToArray());
            }
            return PandaResponse.CreateSuccessResponse(tokenData);

        }

        public async Task<Results<bool>> Logout(long IdentityUser)
        {
            var logOutResult = await _userLogic.Logout(IdentityUser);
            if (!logOutResult)
            {
                return PandaResponse.CreateErrorResponse<bool>("Can not log out.");
            }
            return PandaResponse.CreateSuccessResponse(logOutResult);
        }


        public async Task<Results<RefreshTokenContract>> RefreshToken(RefreshTokenContract token)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(token.Token);
            if (principal == null)
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>("Invalid refresh token");
            }

            var email = _jwtTokenService.GetValueFromClaimType(principal, JwtRegisteredClaimNames.Email);
            if (string.IsNullOrEmpty(email))
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>("Invalid claim email");
            }

            var userId = _jwtTokenService.GetValueFromClaimType(principal, JwtRegisteredClaimNames.NameId);
            if (string.IsNullOrEmpty(userId))
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>("Invalid claim userId");
            }

            var tokenValid = await _userLogic.ValidateRefreshToken(email, token.RefreshToken);
            if (tokenValid.IsError())
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>(tokenValid.Errors.ToArray());
            }

            var newJwtToken = _jwtTokenService.GenerateToken(email, userId);
            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            var updateRefreshTokenResult = await _userLogic.UpdateRefreshTokenToUser(email, newRefreshToken);
            if (updateRefreshTokenResult.IsError())
            {
                return PandaResponse.CreateErrorResponse<RefreshTokenContract>(updateRefreshTokenResult.Errors.ToArray());
            }

            var result = PandaResponse.CreateSuccessResponse<RefreshTokenContract>(
                    new RefreshTokenContract
                    {
                        Token = newJwtToken,
                        RefreshToken = newRefreshToken
                    });
            return result;
        }

        public async Task<Results<PandaUserContract>> GetMyAccount(long IdentityUser)
        {
            return await _userLogic.GetMyAccount(IdentityUser);
        }

        public async Task<Results<List<PandaUserSearchResultContract>>> GetAllAccount(PagingParameters paging)
        {
            return await _userLogic.GetAllAccount(paging);
        }

        public async Task<Results<long>> CreateUser(PandaUserContract newAccount)
        {
            return await _userLogic.CreateUser(newAccount);
        }
    }
}
