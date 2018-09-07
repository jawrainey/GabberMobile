using System.Collections.Generic;
using Newtonsoft.Json;
using SQLite;

namespace GabberPCL.Models
{
    public class Project
    {
        [PrimaryKey]
        public int ID { get; set; }

        [JsonProperty("content")]
        public Dictionary<string, Content> Content;
        public string ContentJson { get; set; }

        [JsonProperty("is_public")]
        public bool IsPublic { get; set; }

        [JsonProperty("default_lang")]
        public int IsDefaultLang { get; set; }

        public Creator Creator;
        public string CreatorJson { get; set; }

        public Organisation Organisation;
        public int OrganisationId { get; set; }
        public string OrganisationJson { get; set; }

        public List<Member> Members;
        public string MembersJson { get; set; }

        public string image { get; set; }

        // only used in iOS collectionview
        public bool IsExpanded;

        public void LoadJson()
        {
            if (CreatorJson == null) return;
            Creator = JsonConvert.DeserializeObject<Creator>(CreatorJson);
            Members = JsonConvert.DeserializeObject<List<Member>>(MembersJson);
            Organisation = JsonConvert.DeserializeObject<Organisation>(OrganisationJson);
            Content = JsonConvert.DeserializeObject<Dictionary<string, Content>>(ContentJson);
        }

        public void SerializeJson()
        {
            CreatorJson = JsonConvert.SerializeObject(Creator);
            MembersJson = JsonConvert.SerializeObject(Members);
            OrganisationJson = JsonConvert.SerializeObject(Organisation);
            ContentJson = JsonConvert.SerializeObject(Content);
        }
    }
}