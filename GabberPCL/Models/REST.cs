using System.Collections.Generic;
using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class Meta
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("messages")]
        public List<string> Messages { get; set; }
    }

    public class ServerResponse<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class DataUserTokens
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("tokens")]
        public JWToken Tokens { get; set; }
    }

    public class CustomLanguagesResponse
    {
        public Meta Meta { get; set; }
        public List<LanguageChoice> Data { get; set; }
    }
}
