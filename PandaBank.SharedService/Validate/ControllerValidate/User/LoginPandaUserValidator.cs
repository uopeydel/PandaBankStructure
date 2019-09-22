using FluentValidation;
using PandaBank.SharedService.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Validate.ControllerValidate.User
{
    public class LoginPandaUserValidator : AbstractValidator<PandaUserLoginContract>
    {

        public LoginPandaUserValidator()
        {
            RuleFor(x => x.Email).NotNull().EmailAddress().Length(6, 128);
            RuleFor(x => x.Password).Length(6, 32);
        }


    }
}
