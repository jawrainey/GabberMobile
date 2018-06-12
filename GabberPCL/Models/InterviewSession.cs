using System;
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
        public string CreatorEmail { get; set; }
        [ForeignKey(typeof(Project))]
        public int ProjectID { get; set; }
        public DateTime CreatedAt { get; set; }
        // Consent type participants agreed to: public, members, or private
        public string ConsentType { get; set; }

        [OneToMany]
        public List<InterviewPrompt> Prompts { get; set; }
        [OneToMany]
        public List<InterviewParticipant> Participants { get; set; }

        // Used for UI
        public bool IsUploaded { get; set; }
        public bool IsUploading { get; set; }
    }
}