using PandaBank.SharedService.Const;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Service.Facade.Interface
{
    public interface IGatewayFacade
    {
        Task<Results<object>> GateWayInvoke<TBody>(
            Enums.HttpMethod method,
            string token,
            string path, 
            TBody body,
            string query = null
            );
    }
}
