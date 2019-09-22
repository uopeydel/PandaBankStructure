using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Read
{
    public class UserAccountReadContract
    {
       
        public long PandaUserId { get; set; } 
        public string PandaAccountId { get; set; }
    }
}
