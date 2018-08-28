using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class Member
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public long Id { get; set; }

        public bool Confirmed { get; set; }
        public bool Deactivated { get; set; }
        public string Role { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("user_id")]
        public long UserId { get; set; }

        [JsonProperty("project_id")]
        public int ProjectID { get; set; }
    }
}