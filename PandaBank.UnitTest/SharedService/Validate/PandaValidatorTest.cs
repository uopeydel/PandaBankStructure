using PandaBank.SharedService.Validate;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PandaBank.UnitTest.SharedService.Validate
{
    public class PandaValidatorTest
    {

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("/", false)]
        [InlineData(" ", false)]
        [InlineData("   @   ", false)]
        [InlineData(" @ ", false)]
        [InlineData("a @a. a", false)]
        [InlineData("a@a", false)]
        [InlineData("a@a .com", false)]
        [InlineData("true", false)]
        [InlineData("a@a.a", false)] 
        [InlineData("$a@a.a", false)]
        [InlineData("a@a.aa", true)]
        [InlineData("aaa@a.aaa", true)]
        [InlineData("a@aa.aa", true)]
        public void EmailValidTest(string email, bool isValid)
        {
            var result = PandaValidator.EmailIsValid(email);
            Assert.Equal(result, isValid);
        }
    }
}
