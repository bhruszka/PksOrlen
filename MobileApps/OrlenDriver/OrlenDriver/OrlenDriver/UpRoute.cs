using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace OrlenDriver
{
    public partial class UpRoute
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("route_ids_json")]
        public string RouteIdsJson { get; set; }

        [JsonProperty("route")]
        public Route[] Route { get; set; }
    }
}
