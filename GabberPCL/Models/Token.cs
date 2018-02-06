﻿using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class JWToken
    {
        [JsonProperty("access_token")]
        public string Access { get; set; }
        [JsonProperty("refresh_token")]
        public string Refresh { get; set; }
    }
}
