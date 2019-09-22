using PandaBank.SharedService.Const;
using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.Account.Create
{
    public class PandaStatementCreateContract
    { 
        public string PandaAccountId { get; set; }  

        public double Balances { get; set; }
        public Enums.PandaStatementStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; }
    }
}
