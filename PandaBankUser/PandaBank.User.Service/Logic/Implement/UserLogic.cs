using AutoMapper;
using Microsoft.AspNetCore.Identity;
using PandaBank.SharedService.Contract;
using PandaBank.SharedService.Contract.User;
using PandaBank.SharedService.Extensions;
using PandaBank.SharedService.Validate;
using PandaBank.User.DAL.Models;
using PandaBank.User.DAL.Repository.Interface;
using PandaBank.User.Service.Logic.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.User.Service.Logic.Implement
{
    public class UserLogic : IUserLogic
    {
        public readonly DateTime now;
        private readonly UserManager<PandaUser> _userManager;
        private readonly SignInManager<PandaUser> _signInManager;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UserLogic(
            UserManager<PandaUser> userManager,
            SignInManager<PandaUser> signInManager,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userRepository = userRepository;
            _mapper = mapper;

            now = DateTime.Now;
        }

        public async Task<Results<PandaUser>> Login(PandaUserLoginContract account)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(account.Email, account.Password, false, false);
            if (signInResult.Succeeded)
            {
                return PandaResponse.CreateSuccessResponse(await _userManager.FindByEmailAsync(account.Email));
            }
            else
            {
                bool emailValid = await _userRepository.EmailValid(account.Email);
                if (!emailValid)
                {
                    return PandaResponse.CreateErrorResponse<PandaUser>("Email inValid.");
                }
                return PandaResponse.CreateErrorResponse<PandaUser>("Password inValid.");
            }
        }

        public async Task<Results<bool>> UpdateRefreshTokenToUser(
            string email,
            string refreshToken
            )
        {
            var updateRefreshTokenResponse = await _userRepository.UpdateRefreshToken(email, refreshToken);
            return updateRefreshTokenResponse;
        }

        public async Task<Results<bool>> ValidateRefreshToken(
            string email,
            string refreshToken
            )
        {
            string savedRefreshToken = await _userRepository.GetRefreshToken(email);
            if (refreshToken != savedRefreshToken)
            {
                return PandaResponse.CreateErrorResponse<bool>("Refresh token Invalid");
            }
            return PandaResponse.CreateSuccessResponse(true);

        }

        public async Task<Results<PandaUserContract>> GetMyAccount(long IdentityUser)
        {
            var pandaUser = await _userManager.FindByIdAsync(IdentityUser.ToString());
            var newPanda = new PandaUserContract();
            _mapper.Map(pandaUser, newPanda);
            if (newPanda != null && pandaUser != null)
            {
                return PandaResponse.CreateSuccessResponse(newPanda);
            }

            return PandaResponse.CreateErrorResponse<PandaUserContract>("User not exist");

        }

        //TODO : Permission
        public async Task<Results<List<PandaUserSearchResultContract>>> GetAllAccount(PagingParameters paging)
        {
            var pandaUsers = await _userRepository.GetAllUser(paging);
            return pandaUsers;
        }

        public async Task<Results<long>> CreateUser(PandaUserContract account)
        {
            var emailValid = PandaValidator.EmailIsValid(account.Email);
            if (!emailValid)
            {
                return PandaResponse.CreateErrorResponse<long>("Email Invalid");
            }

            var user = await _userManager.FindByEmailAsync(account.Email);
            if (user != null)
            {
                return PandaResponse.CreateSuccessResponse(user.Id);
            }

            var newAccount = new PandaUser
            {
                Email = account.Email,
                FirstName = account.FirstName,
                LastName = account.LastName,
                UserName = account.Email,
                CreatedAt = now,
                UpdatedAt = now,
            };

            var result = await _userManager.CreateAsync(newAccount, account.Password);

            if (result.Succeeded)
            {
                return PandaResponse.CreateSuccessResponse(newAccount.Id);
            }
            else
            {
                return PandaResponse.CreateErrorResponse<long>(result.Errors.Select(s => s.Code + " : " + s.Description).ToArray());
            }

        }

        public async Task<bool> RefreshTokenIsNullOrEmpty(string email)
        {
            string savedRefreshToken = await _userRepository.GetRefreshToken(email);
            var isNullOrEmpty = string.IsNullOrEmpty(savedRefreshToken);
            return isNullOrEmpty;
        }

        public async Task<bool> Logout(long identityUser)
        {
            var user = await _userManager.FindByIdAsync(identityUser.ToString());
            if (user == null)
            {
                return false;
            }

            //TODO : make user loged out by another way if frontend lost an token,refresh token
            bool refreshTokenIsClear = await _userRepository.ClearRefreshToken(identityUser);
            return refreshTokenIsClear;
        }
    }
}
