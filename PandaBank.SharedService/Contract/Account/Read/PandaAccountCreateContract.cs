using PandaBank.SharedService.Contract.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Read
{
    public class PandaAccountReadContract
    {  
        public string Id { get; set; } 
        public string Name { get; set; }

        public string Description { get; set; }
          
        public double Balances { get; set; }
         
        public bool Active { get; set; }
         
        public virtual List<PandaStatementReadContract> PandaStatement { get; set; }
        public virtual List<UserAccountReadContract> UserAccounts { get; set; }
    }
}
