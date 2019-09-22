using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Contract
{
    [DataContract]
    public class PandaUserContract : PandaUserLoginContract
    {
        [DataMember]
        public string FirstName { get; set; }
        [DataMember]
        public string LastName { get; set; }
    }

    [DataContract]
    public class PandaUserLoginContract
    {
        [DataMember]
        public long Id { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public string Password { get; set; }
    }


}
