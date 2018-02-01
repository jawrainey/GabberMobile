using System.Collections.Generic;
using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class InterviewSession
    {
        [PrimaryKey]
        public string SessionID { get; set; }
        public string RecordingURL { get; set; }

        [ForeignKey(typeof(User))]
        public int CreatorID { get; set; }
        [ForeignKey(typeof(Project))]
        public int ProjectID { get; set; }

        [OneToMany]
        public List<InterviewPrompt> Prompts { get; set; }
        [OneToMany]
        public List<InterviewParticipant> Participants { get; set; }
        // Used for UI
        public bool IsUploaded { get; set; }
    }
}