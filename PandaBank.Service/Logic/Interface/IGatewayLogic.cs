using PandaBank.SharedService.Const;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Service.Logic.Interface
{
    public interface IGatewayLogic
    {
        Task<Results<object>> Caller(
            Enums.HttpMethod method,
            string token,
            string path,
            string jsonBody = "",
            string query = ""
            );
        string GetQueryString<T>(T obj);
    }
}
