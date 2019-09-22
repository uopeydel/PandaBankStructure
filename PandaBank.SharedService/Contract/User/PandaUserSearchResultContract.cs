using System;
using System.Collections.Generic;
using System.Text;

namespace PandaBank.SharedService.Contract.User
{
    public class PandaUserSearchResultContract
    {
        public long Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        public virtual string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
