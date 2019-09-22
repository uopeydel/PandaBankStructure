using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PandaBank.Service.Facade.Interface;
using PandaBank.Service.Logic.Interface;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PandaBank.Service.Facade.Implement
{
    public class GatewayFacade : IGatewayFacade
    {
        private readonly IGatewayLogic _gatewayLogic;

        public GatewayFacade(IGatewayLogic gatewayLogic)
        {
            _gatewayLogic = gatewayLogic;
        }


        public async Task<Results<object>> GateWayInvoke<TBody>(
            Enums.HttpMethod method,
            string token,
            string path, 
            TBody body,
            string query = ""
            )
        {
            string jsonBody = body == null ? null : JsonConvert.SerializeObject(body);
            var result = await _gatewayLogic.Caller(
                method,
                token,
                path,
                jsonBody, 
                query
                ); 
            return result;  
        }



    }

}
