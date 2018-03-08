using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class Prompt
    {
        [PrimaryKey]
        public int ID { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        public string ImageURL { get; set; }

        [ForeignKey(typeof(User))]
        [JsonProperty("creator_id")]
        public int CreatorID { get; set; }
        [ForeignKey(typeof(Project))]
        [JsonProperty("project_id")]
        public int ProjectID { get; set; }

        // UI attributes
        public bool Selected { get; set; }
        public SelectedState SelectionState { get; set; }
        public enum SelectedState { never, previous, current }
    }
}