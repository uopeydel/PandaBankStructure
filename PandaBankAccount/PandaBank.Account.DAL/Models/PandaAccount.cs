using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PandaBank.Account.DAL.Models
{
    public class PandaAccount
    {
        public PandaAccount()
        {
            UserAccounts = new HashSet<UserAccount>();
            PandaStatement = new HashSet<PandaStatement>();
        }


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MinLength(20)]
        public string Id { get; set; }

        [MinLength(2)]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }

        [Range(0, Double.MaxValue)]
        public double Balances { get; set; }

        //TODO: find real status is must have
        public bool Active { get; set; }


        public virtual ICollection<PandaStatement> PandaStatement { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }


}
