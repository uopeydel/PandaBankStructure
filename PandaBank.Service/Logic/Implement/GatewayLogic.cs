using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using PandaBank.Service.Logic.Interface;
using PandaBank.SharedService.Const;
using PandaBank.SharedService.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PandaBank.Service.Logic.Implement
{
    public class GatewayLogic : IGatewayLogic
    {
        private string hostUrl = "";
        private string pandaSecureKey = "";
        private readonly IConfiguration _configuration;
        public GatewayLogic(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //TODO : beware Overhead of Rest Http 
        public async Task<Results<object>> Caller(
            Enums.HttpMethod method, string token, string path, string jsonBody = "", string query = ""
            )
        {
            var controllerName = path.Replace("/api/", "");
            int index = controllerName.IndexOf("/");
            if (index > 0)
            {
                controllerName = controllerName.Substring(0, index);
            }

            path = path.Replace("/api/", "api/");
            hostUrl = _configuration[$"URLSub:{controllerName}"];

            StringContent stringContent = null;
            pandaSecureKey = _configuration[$"SecureKey:Sub:PandaBank{controllerName}"];

            //ex "https://localhost:44357/api/account/me?queryparam=x&"
            var fullUrl = $"{hostUrl}{path}{query}";

            using (var client = new HttpClient())
            {
                if (!string.IsNullOrEmpty(pandaSecureKey))
                {
                    client.DefaultRequestHeaders.Add("Panda", pandaSecureKey);
                }
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                }
                if (!string.IsNullOrEmpty(jsonBody))
                {
                    stringContent = new StringContent
                        (
                        jsonBody,
                        Encoding.UTF8,
                        "application/json");
                }

                HttpResponseMessage responseMessage = null;
                switch (method)
                {
                    case Enums.HttpMethod.GET:
                        responseMessage = await client.GetAsync(fullUrl);
                        break;
                    case Enums.HttpMethod.POST:
                        responseMessage = await client.PostAsync(fullUrl, stringContent);
                        break;
                    case Enums.HttpMethod.DELETE:
                        responseMessage = await client.DeleteAsync(fullUrl);
                        break;
                    case Enums.HttpMethod.PUT:
                        responseMessage = await client.PutAsync(fullUrl, stringContent);
                        break;
                }

                string resultContent = await responseMessage.Content.ReadAsStringAsync();
                var objectResult = JsonConvert.DeserializeObject<Results<object>>(resultContent);
                return objectResult;
            }
        }


        public string GetQueryString<T>(T obj)
        {
            var properties = obj.GetType().GetProperties()
                .Where(w => w.GetValue(obj, null) != null)
                .Select(s => s.Name + "=" + HttpUtility.UrlEncode(s.GetValue(obj, null).ToString()))
                .ToList();
            return string.Join("&", properties.ToArray());
            //var properties = from p in obj.GetType().GetProperties()
            //                 where p.GetValue(obj, null) != null
            //                 select p.Name + "=" + HttpUtility.UrlEncode(p.GetValue(obj, null).ToString()); 
            //return string.Join("&", properties.ToArray());
        }

    }
}
