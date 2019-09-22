using PandaBank.SharedService.Const;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Read
{
    public class PandaStatementReadContract
    { 
        public string PandaAccountId { get; set; }  

        public double Balances { get; set; }
        public Enums.PandaStatementStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
