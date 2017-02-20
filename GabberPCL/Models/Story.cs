using SQLite;

namespace GabberPCL
{
	public class Story
	{
		[PrimaryKey]
		// The filepath to the audio experience.
		public string AudioPath { get; set; }
		// The filepath to a photo of the interviewee.
		public string PhotoPath { get; set; }
		public string InterviewerEmail { get; set; }
		public string IntervieweeEmail { get; set; }
		public string IntervieweeName { get; set; }
		public string IntervieweeGender { get; set; }
		public string IntervieweeAge { get; set; }
		public string Location { get; set; }
		public string promptText { get; set; }
		public string ComplexNeedsAsJSON { get; set; }
		public bool Uploaded { get; set; }
	}
}