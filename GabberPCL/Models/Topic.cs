using Newtonsoft.Json;

namespace GabberPCL.Models
{
    public class Topic
    {
        public int ID { get; set; }
        [JsonProperty("lang_id")]
        public int LangID { get; set; }
        [JsonProperty("project_id")]
        public int ProjectID { get; set; }
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
        public string Text { get; set; }

        // UI attributes
        public bool Selected { get; set; }
        public SelectedState SelectionState { get; set; }
        public enum SelectedState { never, previous, current }
    }
}