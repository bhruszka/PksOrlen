using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class UpNode
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("node_1")]
        public Node Node1 { get; set; }

        [JsonProperty("node_2")]
        public Node Node2 { get; set; }

        [JsonProperty("distance")]
        public long Distance { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

        [JsonProperty("has_bus_stop")]
        public bool HasBusStop { get; set; }

        [JsonProperty("max_height")]
        public object MaxHeight { get; set; }

        [JsonProperty("max_width")]
        public object MaxWidth { get; set; }

        [JsonProperty("open")]
        public bool Open { get; set; }

        [JsonProperty("max_weight")]
        public object MaxWeight { get; set; }
    }
}
