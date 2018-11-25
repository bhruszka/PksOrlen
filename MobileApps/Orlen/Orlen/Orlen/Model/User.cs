using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class User
    {
        [JsonProperty("username")]
        public string Login { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
