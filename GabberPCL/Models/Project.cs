using System.Collections.Generic;
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
        public int IsPublic { get; set; }
        public int HasConsent { get; set; }

        [OneToMany]
        public List<Prompt> Prompts { get; set; }
    }
}