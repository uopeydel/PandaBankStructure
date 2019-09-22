using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PandaBank.Account.DAL.Models
{
    public class UserAccount
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long PandaUserId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string PandaAccountId { get; set; }
        [ForeignKey("PandaAccountId")]
        public virtual PandaAccount PandaAccount { get; set; }


    }
}
