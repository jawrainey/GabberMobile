using System.Collections.Generic;
using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class Content
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        [JsonProperty("lang_id")]
        public int LangID { get; set; }
        public List<Topic> Topics;
    }
}
