using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PandaBank.User.DAL.Models
{

    public class PandaUser : IdentityUser<long>
    {
        [Required]
        [StringLength(200)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(200)]
        public string LastName { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime UpdatedAt { get; set; }


        public string RefreshToken { get; set; }
        [Column(TypeName = "datetime2")]
        public DateTime TokenExpiredAt { get; set; }

    }

    #region NotImplement

    public class PandaUserLogin : IdentityUserLogin<long>
    {

    }


    public class PandaUserRole : IdentityUserRole<long>
    {

    }


    public class PandaUserClaim : IdentityUserClaim<long>
    {

    }


    public class PandaRoleClaim : IdentityRoleClaim<long>
    {

    }


    public class PandaUserToken : IdentityUserToken<long>
    {

    }

    public class PandaRole : IdentityRole<long>
    {

    }


    #endregion

}
