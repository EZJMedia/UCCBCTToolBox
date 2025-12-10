using Newtonsoft.Json;
using System;

namespace UCCBCTToolBox.Models
{
    internal class Device
    {
        [JsonProperty ("id")]
        public int Id { get; set; }

        [JsonProperty ("user_id")]
        public int UserId { get; set; }

        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonProperty ("hash")]
        public string Hash { get; set; }

        [JsonProperty ("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty ("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}
