using SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;

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

	// As a request is made each time, simplify by storing as JSON.
	public class ProjectsAsJSON
	{
		[PrimaryKey]
		public int Id { get; set; }
		public string Json { get; set; }
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
			database.CreateTable<ProjectsAsJSON>();
		}

		public void SaveRequest(string json)
		{
			// Has a request been made and stored?
			var jsonExists = database.Table<ProjectsAsJSON>().Where(row => row.Id == 1).ToString();
			var oneRowToRuleThemAll = new ProjectsAsJSON { Json = json, Id = 1 };
			// Do not create a new row when a request comes in
			if (!string.IsNullOrEmpty(jsonExists)) database.Insert(oneRowToRuleThemAll);
			else database.Update(oneRowToRuleThemAll);
		}

		public List<Project> GetProjects()
		{
			// TODO: what if after _inital_ login no request can be made?
			var query = database.Table<ProjectsAsJSON>().First().Json;
			return JsonConvert.DeserializeObject<RootObject>(query).projects;
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