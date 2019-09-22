using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Create
{
    public class UserAccountCreateContract
    {
       
        public long PandaUserId { get; set; } 
        public string PandaAccountId { get; set; }
    }
}
