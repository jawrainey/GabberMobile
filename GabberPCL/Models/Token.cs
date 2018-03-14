using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class JWToken
    {
        [JsonProperty("access")]
        public string Access { get; set; }
        [JsonProperty("refresh")]
        public string Refresh { get; set; }
    }
}
