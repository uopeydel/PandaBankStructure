using FluentValidation;
using PandaBank.SharedService.Contract;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Validate.ControllerValidate.User
{
    public class RefreshTokenValidator : AbstractValidator<RefreshTokenContract>
    {

        public RefreshTokenValidator()
        {
            RuleFor(x => x.Token).NotNull();
            RuleFor(x => x.RefreshToken).NotNull();
        }

    }

}
