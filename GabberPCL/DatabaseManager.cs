using SQLite;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace GabberPCL
{
	public class DatabaseManager
	{
		public SQLiteConnection database;

		public DatabaseManager(string folder)
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
			return JsonConvert.DeserializeObject<List<Project>>(queryResult);
		}

		public void UpdateStory(Story story)
		{
			database.Update(story);
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