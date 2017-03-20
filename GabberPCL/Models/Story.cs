using SQLite;

namespace GabberPCL
{
	public class Story
	{
		[PrimaryKey]
		public string AudioPath { get; set; }
		public string Location { get; set; }
		public string promptText { get; set; }
		// Note: this is only ever used locally to differentiate between users as it's based on login details 
		public string InterviewerEmail { get; set; }
		// lazy hack: required to know which participants were part of this interview. 
		// Simplifies data access, e.g. no need for relationships, etc
		public string ParticipantsAsJSON { get; set; }
		public bool Uploaded { get; set; }
		public string SessionID { get; set; }
	}
}