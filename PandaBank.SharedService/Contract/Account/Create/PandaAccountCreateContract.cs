using PandaBank.SharedService.Contract.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Create
{
    public class PandaAccountCreateContract
    {  
        public string Id { get; set; } 
        public string Name { get; set; }

        public string Description { get; set; }
          
        public double Balances { get; set; }
         
        public bool Active { get; set; }
         
        public virtual List<PandaStatementCreateContract> PandaStatement { get; set; }
        public virtual List<UserAccountCreateContract> UserAccounts { get; set; }
    }
}
