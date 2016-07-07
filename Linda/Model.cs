using SQLite;
using System.Collections.Generic;
using System.Linq;

namespace Linda
{
	// A user table is not required as we can perform a lookup where the logged in email is equal
	// to InterviewerEmail to acquire all the audios (in this device) that have been recorded.
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
		// The Latitude & longitude stored as: "LAT, LONG"
		public string Location { get; set; }
		public string promptText { get; set; }
		// Whether or not this story has been uploaded to the web.
		public bool Uploaded { get; set; }
	}

	public class Model
	{
		public SQLiteConnection database;

		public Model()
		{
			string path = System.IO.Path.Combine(
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal),
				"database.db3");
			database = new SQLiteConnection(path);
			database.CreateTable<Story>();
		}

		public void InsertStory(Story story)
		{
			database.Insert(story);
		}

		public List<Story> GetStories(string interviewer)
		{
			return database.Table<Story>().Where(row => row.InterviewerEmail == interviewer).ToList();
		}
	}
}