using Orlen.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Orlen.Services
{
    class OrlenWebService : WebService
    {
        public async Task<List<TimeTableLineItem>> GetBusStopsAsync(string Token)
        {
            var url = new Uri(_baseUri, new Uri(QueryBuilderGetBusStopsList()));
            var _headers = new Dictionary<string, string>
            {
                { "Authorization:", "Token " + Token }
            };
            var data = await SendRequestAsyncWithDataResultList<UpNode[]>(url, httpMethod: HttpMethod.Get, headers: _headers);
            var list = new List<TimeTableLineItem>();
            foreach(var item in data)
            {
                var temp = new TimeTableLineItem()
                {
                    Lat = (item.Node1.Latitude + item.Node2.Latitude) / 2,
                    Long = (item.Node1.Longitude + item.Node2.Longitude) / 2
                };
                list.Add(temp);
            }
            return list;
        }

        private string QueryBuilderGetBusStopsList()
        {
            var sb = new System.Text.StringBuilder("/api/edges/").Append("?has_bus_stop=").Append("true");

            return sb.ToString();
        }
    }
}
