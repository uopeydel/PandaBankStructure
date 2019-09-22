using Moq;
using PandaBank.Account.DAL.Models;
using PandaBank.Account.Service.Facade.Implement;
using PandaBank.Account.Service.Logic.Interface;
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
    public class AccountFacadeTest
    {
        private readonly Mock<IAccountLogic> _accountLogicMocking;
        private readonly DateTime now = DateTime.Now;
        private readonly AccountFacade accountFacade;


        private const string accountIdSuccess = "123456789012345678901234567890123456";
        private const string accountIdFail = "fail";


        private const long userIdSuccess = 1;
        private const long userIdFail = 0;

        private const int indexFail = 1;
        private const int indexPass = 0;

        public AccountFacadeTest()
        {
            var mock = new MockRepository(MockBehavior.Default);
            _accountLogicMocking = new Mock<IAccountLogic>();

            #region CreateAccount 

            _accountLogicMocking
                .Setup(s => s.GenerateAccountId())
                .Returns(Task.FromResult(accountIdSuccess));

            var testDataPass = PandaAccountCreateContractTestData.pandaAccountCreateContracts[indexPass];
            var testDataFail = PandaAccountCreateContractTestData.pandaAccountCreateContracts[indexFail];
            _accountLogicMocking
                .Setup(s => s.CreateAccount(testDataPass))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));

            _accountLogicMocking
                .Setup(s => s.CreateAccount(testDataFail))
                .Returns(Task.FromResult(PandaResponse.CreateErrorResponse<bool>("test")));

            #endregion

            #region DeleteAccount

            _accountLogicMocking
                .Setup(s => s.UnActiveAccount(userIdSuccess, accountIdSuccess))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));
            _accountLogicMocking
                .Setup(s => s.UnActiveAccount(userIdSuccess, accountIdFail))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));
            _accountLogicMocking
                .Setup(s => s.UnActiveAccount(userIdFail, accountIdSuccess))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));
            _accountLogicMocking
                .Setup(s => s.UnActiveAccount(userIdFail, accountIdFail))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));

            #endregion


            #region UpdateStatement

            _accountLogicMocking
                .Setup(s => s.CanActiveAccount(userIdSuccess, accountIdSuccess))
                .Returns(Task.FromResult(true));
            _accountLogicMocking
                .Setup(s => s.CanActiveAccount(userIdSuccess, accountIdFail))
                .Returns(Task.FromResult(false));
            _accountLogicMocking
                .Setup(s => s.CanActiveAccount(userIdFail, accountIdSuccess))
                .Returns(Task.FromResult(false));
            _accountLogicMocking
               .Setup(s => s.CanActiveAccount(userIdFail, accountIdFail))
               .Returns(Task.FromResult(false));


            _accountLogicMocking
                .Setup(s => s.CanUpdateBalance(testDataPass.PandaStatement[0].PandaAccountId, testDataPass.PandaStatement[0].Balances, It.IsAny<Enums.PandaStatementStatus>()))
                .Returns(Task.FromResult(true));

            _accountLogicMocking
                .Setup(s => s.CanUpdateBalance(testDataPass.PandaStatement[0].PandaAccountId, testDataFail.PandaStatement[0].Balances, It.IsAny<Enums.PandaStatementStatus>()))
                .Returns(Task.FromResult(false));

            _accountLogicMocking
                .Setup(s => s.CanUpdateBalance(testDataFail.PandaStatement[0].PandaAccountId, testDataPass.PandaStatement[0].Balances, It.IsAny<Enums.PandaStatementStatus>()))
                .Returns(Task.FromResult(false));

            _accountLogicMocking
                .Setup(s => s.CanUpdateBalance(testDataFail.PandaStatement[0].PandaAccountId, testDataFail.PandaStatement[0].Balances, It.IsAny<Enums.PandaStatementStatus>()))
                .Returns(Task.FromResult(false));


            _accountLogicMocking
                .Setup(s => s.RunStatement(testDataPass.PandaStatement[0]))
                .Returns(Task.FromResult(PandaResponse.CreateSuccessResponse(true)));
            _accountLogicMocking
               .Setup(s => s.RunStatement(testDataFail.PandaStatement[0]))
               .Returns(Task.FromResult(PandaResponse.CreateErrorResponse<bool>("test")));
            #endregion


            accountFacade = new AccountFacade(
                _accountLogicMocking.Object);
        }

        [Theory]
        [InlineData(userIdSuccess, indexPass, false)]
        [InlineData(userIdSuccess, indexFail, true)]
        [InlineData(userIdFail, indexPass, true)]
        [InlineData(userIdFail, indexFail, true)]
        public async Task CreateAccount(int identityUser, int index, bool result)
        {
            var testData = PandaAccountCreateContractTestData.pandaAccountCreateContracts[index];
            var runStatementResult = await accountFacade.CreateAccount(identityUser, testData);
            Assert.Equal(runStatementResult.IsError(), result);
        }

        [Theory]
        [InlineData(userIdSuccess, accountIdSuccess, true)]
        [InlineData(userIdSuccess, accountIdFail, false)]
        [InlineData(userIdFail, accountIdSuccess, false)]
        [InlineData(userIdFail, accountIdFail, false)]
        public async Task DeleteAccount(int identityUser, string accountId, bool result)
        {

            var runStatementResult = await accountFacade.DeleteAccount(identityUser, accountId);
            Assert.Equal(runStatementResult.IsError(), result);
        }


        [Theory]
        [InlineData(userIdSuccess, indexPass, false)]
        [InlineData(userIdSuccess, indexFail, true)]
        [InlineData(userIdFail, indexPass, true)]
        [InlineData(userIdFail, indexFail, true)]
        public async Task UpdateStatement(int identityUser, int index, bool result)
        {
            var testData = PandaAccountCreateContractTestData.pandaAccountCreateContracts[index];
            var runStatementResult = await accountFacade.UpdateStatement(identityUser, testData.PandaStatement[0], Enums.PandaStatementStatus.Deposit);
            Assert.Equal(runStatementResult.IsError(), result);
        }


    }


    public static class PandaAccountCreateContractTestData
    {
        public static List<PandaAccountCreateContract> pandaAccountCreateContracts = new List<PandaAccountCreateContract>()
        {
            new PandaAccountCreateContract{
                Active = true,
                Balances =0 ,
                Description ="",
                Name ="success",
                Id = "123456789012345678901234567890123456",
                PandaStatement = new List<PandaStatementCreateContract>
                {
                    new PandaStatementCreateContract
                    {
                        PandaAccountId = "123456789012345678901234567890123456",
                        Balances = 1,

                    }
                }
            },
            new PandaAccountCreateContract{
                Active = true,
                Balances =0 ,
                Description ="",
                Name ="fail",
                Id = "fail",
                PandaStatement = new List<PandaStatementCreateContract>
                {
                    new PandaStatementCreateContract
                    {
                        PandaAccountId = "fail",
                        Balances = 0,
                    }
                }
            }
        };

    }
}
