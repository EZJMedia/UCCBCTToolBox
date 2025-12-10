using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCCBCTToolBox.Models
{
    internal class ApiResponse
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("payload")]
        public Device Device { get; set; }
    }
}
