using SQLite;
using System.Collections.Generic;

namespace Gabber
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
		// The Latitude & longitude stored as: "LAT, LONG"
		public string Location { get; set; }
		public string promptText { get; set; }
		// Has the story been uploaded?
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

		public void UpdateStory(Story story)
		{
			database.Update(story);
		}

		public void InsertStory(Story story)
		{
			database.Insert(story);
		}
	}

	// These classes are used JSON deserialization.

	public class RootObject
	{
		public List<Project> projects { get; set; }
	}

	public class Project
	{
		public string theme { get; set; }
		public List<Prompt> prompts { get; set; }
	}

	public class Prompt
	{
		public string prompt { get; set; }
		public string imageName { get; set; }
	}
}