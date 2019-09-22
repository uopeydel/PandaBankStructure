using FluentValidation;
using PandaBank.SharedService.Contract.Account.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Validate.ControllerValidate.Account
{
    public class StatementValidator : AbstractValidator<PandaStatementCreateContract>
    {

        public StatementValidator()
        {
            RuleFor(x => x.PandaAccountId).NotNull();
            RuleFor(x => x.Balances).NotEqual(0);
            When(w => w.Status == Const.Enums.PandaStatementStatus.Deposit,
                () =>
                {
                    RuleFor(x => x.Balances).GreaterThan(0);
                });
            When(w => w.Status == Const.Enums.PandaStatementStatus.Witdraw,
              () =>
              {
                  RuleFor(x => x.Balances).LessThan(0);
              });

        }

    }
}
