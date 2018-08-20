using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GabberPCL.Models
{
    public class InterviewParticipant
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        // Consent is private by default (0), and can currently only be configured
        // on the website by each participant for each interview they were part of.
        public int Consent { get; set; }
        // True if the participant was the interviewer, otherwise False
        public bool Role { get; set; }

        public int UserID { get; set; }
        public string InterviewID { get; set; }
    }
}