using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orlen.Model
{
    public class Node
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }

        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("adjacent_nodes")]
        public long[] AdjacentNodes { get; set; }

        [JsonProperty("is_gate")]
        public bool IsGate { get; set; }

        [JsonProperty("turn_radius")]
        public object TurnRadius { get; set; }
    }
}
