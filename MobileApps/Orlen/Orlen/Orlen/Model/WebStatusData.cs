using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class WebStatusData
    {
        [JsonProperty("status")]
        public string Status { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("client_id")]
        public string Client_id { get; set; }
        [JsonProperty("user_id")]
        public string User_id { get; set; }
        [JsonProperty("errcode")]
        public string Errcode { get; set; }
        [JsonProperty("errmsg")]
        public string Errmsg { get; set; }
        [JsonProperty("id")]
        public int ID { get; set; }

    }
}
