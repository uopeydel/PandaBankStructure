using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Contract
{
    public class RefreshTokenContract
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
