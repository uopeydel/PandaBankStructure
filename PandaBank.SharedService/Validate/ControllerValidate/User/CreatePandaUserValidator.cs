using FluentValidation;
using PandaBank.SharedService.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Validate.ControllerValidate.User
{
    public class CreatePandaUserValidator : AbstractValidator<PandaUserContract>
    {

        public CreatePandaUserValidator()
        {
            RuleFor(x => x.Email).NotNull().EmailAddress().Length(6, 128);
            RuleFor(x => x.FirstName).Length(2, 32);
            RuleFor(x => x.LastName).Length(2, 32);
            RuleFor(x => x.Password).Length(6, 32);
        }


    }
}
