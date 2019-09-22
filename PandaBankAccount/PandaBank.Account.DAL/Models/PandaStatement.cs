using PandaBank.SharedService.Const;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PandaBank.Account.DAL.Models
{
    public class PandaStatement
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Key]
        [Column(Order = 1)]
        public string PandaAccountId { get; set; }
        [ForeignKey("PandaAccountId")]
        public virtual PandaAccount PandaAccount { get; set; }

        public double Balances { get; set; }
        public Enums.PandaStatementStatus Status { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }
    }
}
