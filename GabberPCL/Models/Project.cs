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

        [ForeignKey(typeof(Creator))]
        [JsonProperty("creator_id")]
        public int CreatorId { get; set; }

        // Although a creator can create multiple projects, it is not possible
        // to have a OneToMany and automatically populate the creator table
        // without rewriting the deserialiser that is used in the Rest Client.
        [OneToOne]
        public Creator Creator { get; set; }

        [ForeignKey(typeof(Organisation))]
        [JsonProperty("organisation_id", NullValueHandling = NullValueHandling.Ignore)]
        public int OrganisationId { get; set; }

        // This is unfortunatly the same issue as above: because we are using a
        // class as a type it must have a relationship. In this case it should be
        // ManyToOne as a project can have one organisation, but an org can be 
        // associated with many projects. As above, this would require populating
        // the organisation inside Queries.cs:AddProjects however, this takes
        // a list of projects as input, hence a larger re-write would be required.
        [OneToOne]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Organisation Organisation { get; set; }

        [OneToMany]
        public List<Member> Members { get; set; }
    }
}