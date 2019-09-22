using FluentValidation;
using PandaBank.SharedService.Contract.Account.Create;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Validate.ControllerValidate.Account
{
    public class CreateAccountValidator : AbstractValidator<PandaAccountCreateContract>
    {

        public CreateAccountValidator()
        {
            RuleFor(x => x.Balances).NotNull();
            RuleFor(x => x.Name).NotNull();
            RuleFor(x => x.PandaStatement).NotNull();
        }

    }
}
