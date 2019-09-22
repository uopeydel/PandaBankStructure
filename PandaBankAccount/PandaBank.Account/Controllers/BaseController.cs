using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PandaBank.Account.Controllers
{
    public class BaseController : Controller
    {
        protected long IdentityUser
        {
            get
            {
                if (User == null || User.Identity == null)
                {
                    return 0;
                }
                var x = User.Identity as ClaimsIdentity;
                var email = x.Claims.Where(w => w.Type == ClaimTypes.Email).Select(s => s.Value).FirstOrDefault();
                var userId = x.Claims.Where(w => w.Type == ClaimTypes.NameIdentifier).Select(s => s.Value).FirstOrDefault();
                //var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                return long.Parse(userId);
            }
        }


        protected string BaseApiUrl
        {
            get
            {
                var origin = Request.Headers.Where(w => w.Key.Equals("Origin")).Select(s => s.Value).FirstOrDefault();
                if (!string.IsNullOrEmpty(origin)/* && ConfigurationManager.AppSettings["corsOrigins"].Contains(origin)*/)
                {
                    return origin;
                }
                return string.Empty;
            }
        }

        public string GetIp()
        {
            return GetClientIp();
        }

        private string GetClientIp(HttpRequestMessage request = null)
        {
            var remoteIpAddress = HttpContext?.Features?.Get<IHttpConnectionFeature>()?.RemoteIpAddress;
            return remoteIpAddress?.ToString();
        }

    }
}
