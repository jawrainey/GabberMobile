using System;
using System.Collections.Generic;
using Newtonsoft.Json;
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
        public int ProjectID { get; set; }
        public DateTime CreatedAt { get; set; }

        // Consent type participants agreed to: public, members, or private
        public string ConsentType { get; set; }

        public List<InterviewPrompt> Prompts;
        public string PromptsJson { get; set; }

        public List<InterviewParticipant> Participants;
        public string ParticipantsJson { get; set; }

        // Used for UI
        public bool IsUploaded { get; set; }
        public bool IsUploading { get; set; }

        public void LoadJson()
        {
            Prompts = JsonConvert.DeserializeObject<List<InterviewPrompt>>(PromptsJson);
            Participants = JsonConvert.DeserializeObject< List<InterviewParticipant>>(ParticipantsJson);
        }

        public void SerializeJson()
        {
            PromptsJson = JsonConvert.SerializeObject(Prompts);
            ParticipantsJson = JsonConvert.SerializeObject(Participants);
        }

    }
}