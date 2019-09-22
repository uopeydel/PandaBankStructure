using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace PandaBank.SharedService.Validate
{
    public static class PandaValidator
    {
        public static bool EmailIsValid(string email)
        {
            try
            {
             
                var emailIsnullOrEmpty = string.IsNullOrEmpty(email);
                if (emailIsnullOrEmpty)
                {
                    return false; 
                }
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != email)
                {
                    return false;
                }
                var validateAtt = new EmailAddressAttribute().IsValid(email);
                if (!validateAtt)
                {
                    return false;
                }
                var isLengthMoreOrEqualThan6 = email?.Length >= 6;
                if (!isLengthMoreOrEqualThan6)
                {
                    return false;
                }
                var firstCharacterIsLetter = char.IsLetter(email.FirstOrDefault());
                if(!firstCharacterIsLetter)
                {
                    return false;
                }
                return (addr.Address == email) && (!emailIsnullOrEmpty) && validateAtt && isLengthMoreOrEqualThan6;
            }
            catch
            {
                return false;
            }
        }

    }
}
