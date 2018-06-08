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
        public string Description { get; set; }
        [JsonProperty("is_public")]
        public bool IsPublic { get; set; }

        [OneToMany]
        [JsonProperty("topics")]
        public List<Prompt> Prompts { get; set; }

        [ForeignKey(typeof(User))]
        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        [OneToOne(CascadeOperations = CascadeOperation.CascadeInsert)]
        [JsonProperty("creator")]
        public User Creator { get; set; }
    }
}