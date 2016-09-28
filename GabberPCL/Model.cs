using SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;

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
		public string Location { get; set; }
		public string promptText { get; set; }
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

		public Model(string folder)
		{
			string path = System.IO.Path.Combine(folder, "database.db3");
			database = new SQLiteConnection(path);
			database.CreateTable<Story>();
			database.CreateTable<ProjectsAsJSON>();
		}

		public void SaveRequest(string json)
		{
			var oneRowToRuleThemAll = new ProjectsAsJSON { Json = json, Id = 1 };
			// Do not create a new row when a request comes in
			if (database.Table<ProjectsAsJSON>().Count() <= 0) database.Insert(oneRowToRuleThemAll);
			else database.Update(oneRowToRuleThemAll);
		}

		public List<Project> GetProjects()
		{
			var queryResult = database.Table<ProjectsAsJSON>().Where(row => row.Id == 1).First().Json;
			// What if after login there is no network? Need to prevent nullable response from first.
			return JsonConvert.DeserializeObject<RootObject>(queryResult).projects;
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