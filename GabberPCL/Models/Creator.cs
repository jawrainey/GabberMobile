using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class Creator
    {
        [PrimaryKey]
        [JsonProperty("user_id")]
        public int UserId { get; set; }
        [JsonProperty("fullname")]
        public string Name { get; set; }
    }
}