using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PandaBank.SharedService.Const
{
    public static class Enums
    {
        public enum HttpMethod
        {
            GET = 0,
            PUT = 1,
            DELETE = 2,
            POST = 3,

            HEAD = 4,
            TRACE = 5,
            PATCH = 6,
            CONNECT = 7,
            OPTIONS = 8,
            CUSTOM = 9,
            NONE = 255,
        }

        public enum PandaStatementStatus
        {
            Deposit = 1,
            Witdraw = 2,
        }

        public static T ToEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }


}
