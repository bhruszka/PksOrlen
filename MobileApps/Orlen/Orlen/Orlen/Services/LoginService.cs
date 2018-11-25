using Orlen.Interfaces;
using Orlen.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Orlen.Services
{
    public class LoginService : WebService, ILoginService
    {
        public async Task<WebStatusData> LoginAsync(User user)
        {
            var url = new Uri(_baseUri + "/api-token-auth/", UriKind.Absolute);
            return await SendRequestAsync<WebStatusData>(url, httpMethod: HttpMethod.Post, requestData: user);
        }
    }
}
