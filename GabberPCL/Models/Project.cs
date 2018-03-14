using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class Project
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Description { get; set; }
        public int CreatorID { get; set; }
        [JsonProperty("is_active")]
        public bool IsPublic { get; set; }

        [OneToMany]
        [JsonProperty("topics")]
        public List<Prompt> Prompts { get; set; }
    }
}