using Newtonsoft.Json;
using SQLite;

namespace GabberPCL.Models
{
    // This represents a conversation participant or/and project owner.
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        // Private methods are not (de)serialized; setting Id with a different
        // property, which is returned in Project.creator
        [JsonProperty("user_id")]
        int UserId { get; set; }
        [JsonProperty("fullname")]
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        [JsonProperty("lang")]
        public int Lang { get; set; }
        public int AppLang { get; set; }
        // This is the view model and not data model
        public bool Selected { get; set; }

        public int Society { get; set; }
        public int AgeBracket { get; set; }
        public int Role { get; set; }
        public int GenderId { get; set; }
        public string GenderTerm { get; set; }
    }
}