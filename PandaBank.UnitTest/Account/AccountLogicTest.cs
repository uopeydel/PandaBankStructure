using AutoMapper;
using Moq;
using PandaBank.Account.DAL.Mapper;
using PandaBank.Account.DAL.Models;
using PandaBank.Account.DAL.Repository.Interface;
using PandaBank.Account.Service.Logic.Implement;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Contract.Account.Create;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PandaBank.UnitTest.Account
{
    public class AccountLogicTest
    {
        private readonly Mock<IAccountRepository> _accountRepository;
        private readonly IMapper _mapper;
        private readonly DateTime now = DateTime.Now;
        private readonly AccountLogic accountLogic;

        private const string AccountIdSuccess = "success";
        private const string AccountIdFail = "fail";
        private const long UserIdSuccess = 1;
        private const long userIdFail = 0;


        private const string accountHundred = "100";
        private const double balanceHundred = 100;

        private const string accountThousand = "1000";
        private const double balanceThousand = 1000;

        public AccountLogicTest()
        {
            var mock = new MockRepository(MockBehavior.Default);
            _accountRepository = new Mock<IAccountRepository>();

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            _mapper = mappingConfig.CreateMapper();


            #region CanActiveAccount 

            _accountRepository
                .Setup(s => s.GetAccountStatus(AccountIdSuccess))
                .Returns(Task.FromResult(true));

            _accountRepository
              .Setup(s => s.GetAccountStatus(AccountIdFail))
              .Returns(Task.FromResult(false));

            _accountRepository
                .Setup(s => s.IsPaticipant(AccountIdSuccess, UserIdSuccess))
                .Returns(Task.FromResult(true));

            _accountRepository
                .Setup(s => s.IsPaticipant(AccountIdSuccess, userIdFail))
                .Returns(Task.FromResult(false));

            _accountRepository
                .Setup(s => s.IsPaticipant(AccountIdFail, UserIdSuccess))
                .Returns(Task.FromResult(false));
            _accountRepository
                .Setup(s => s.IsPaticipant(AccountIdFail, userIdFail))
                .Returns(Task.FromResult(false));

            #endregion

            #region CanUpdateBalance

            _accountRepository
               .Setup(s => s.GetAccountBalance(accountHundred))
               .Returns(Task.FromResult(balanceHundred));

            _accountRepository
               .Setup(s => s.GetAccountBalance(accountThousand))
               .Returns(Task.FromResult(balanceThousand));

            #endregion

            #region RunStatement

            var test0 = PandaStatementCreateContractTestData.pandaStatementCreateContracts(now)[0];
            var new0 = new PandaStatement();
            _mapper.Map(test0, new0);

            var test1 = PandaStatementCreateContractTestData.pandaStatementCreateContracts(now)[1];
            var new1 = new PandaStatement();
            _mapper.Map(test1, new1);

            _accountRepository
              .Setup(s => s.CreateStatement(It.Is<PandaStatement>(m => 
              m.Balances == new0.Balances
              &&
              m.CreatedAt == new0.CreatedAt
              &&
              m.Id == new0.Id
              &&
              m.Status == new0.Status
              )))
              .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));

            _accountRepository
               .Setup(s => s.UpdateAccountBalance(new0.PandaAccountId, new0.Balances))
               .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));

            _accountRepository
            .Setup(s => s.CreateStatement(new1))
            .Returns(Task.FromResult(PandaResponse.CreateErrorResponse<bool>("test error")));

            _accountRepository
               .Setup(s => s.UpdateAccountBalance(new1.PandaAccountId, new1.Balances))
               .Returns(Task.FromResult(PandaResponse.CreateErrorResponse<bool>("test error")));

            #endregion

            accountLogic = new AccountLogic(
                _accountRepository.Object,
                _mapper);
        }

        [Theory]
        [InlineData(userIdFail, AccountIdFail, false)]
        [InlineData(userIdFail, AccountIdSuccess, false)]
        [InlineData(UserIdSuccess, AccountIdFail, false)]
        [InlineData(UserIdSuccess, AccountIdSuccess, true)]
        public async Task CanActiveAccount(long identityUser, string PandaAccountId, bool result)
        {
            var canAvtiveResult = await accountLogic.CanActiveAccount(identityUser, PandaAccountId);
            Assert.Equal(canAvtiveResult, result);
        }


        [Theory]
        [InlineData(accountHundred, balanceHundred, Enums.PandaStatementStatus.Deposit, true)]
        [InlineData(accountThousand, balanceThousand, Enums.PandaStatementStatus.Deposit, true)]
        [InlineData(accountThousand, -balanceThousand, Enums.PandaStatementStatus.Deposit, false)]
        [InlineData(accountHundred, -1, Enums.PandaStatementStatus.Deposit, false)]
        [InlineData(accountHundred, 0, Enums.PandaStatementStatus.Deposit, false)]

        [InlineData(accountHundred, balanceHundred, Enums.PandaStatementStatus.Witdraw, false)]
        [InlineData(accountThousand, balanceThousand, Enums.PandaStatementStatus.Witdraw, false)]
        [InlineData(accountHundred, -balanceHundred, Enums.PandaStatementStatus.Witdraw, true)]
        [InlineData(accountThousand, -balanceThousand, Enums.PandaStatementStatus.Witdraw, true)]
        [InlineData(accountHundred, -balanceThousand, Enums.PandaStatementStatus.Witdraw, false)]
        [InlineData(accountHundred, -1, Enums.PandaStatementStatus.Witdraw, true)]
        [InlineData(accountHundred, 0, Enums.PandaStatementStatus.Witdraw, false)]

        public async Task CanUpdateBalance(
            string pandaAccountId,
            double balancesForUpdate,
            Enums.PandaStatementStatus status,
            bool result)
        {
            var canUpdateResult = await accountLogic.CanUpdateBalance(pandaAccountId, balancesForUpdate, status);

            Assert.Equal(canUpdateResult, result);
        }



        [Theory]
        [InlineData(0, false)]
        [InlineData(1, true)]
        public async Task RunStatement(int index, bool result)
        {
            var testData = PandaStatementCreateContractTestData.pandaStatementCreateContracts(now)[index];
            var runStatementResult = await accountLogic.RunStatement(testData);
            Assert.Equal(runStatementResult.IsError(), result);
        }



    }


    public static class PandaStatementCreateContractTestData
    {
        public static List<PandaStatementCreateContract> pandaStatementCreateContracts(DateTime dateTime) => new List<PandaStatementCreateContract>()
        {
            new PandaStatementCreateContract
            {
                Balances = 100,
                PandaAccountId = "100",
                Status = Enums.PandaStatementStatus.Deposit,
                CreatedAt   = dateTime,
            },
            new PandaStatementCreateContract
            {
                Balances = 100,
                PandaAccountId = "",
                Status = Enums.PandaStatementStatus.Deposit,
                CreatedAt   = dateTime,
            }
        };


    }
}
