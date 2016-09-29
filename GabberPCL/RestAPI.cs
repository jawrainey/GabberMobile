using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

namespace GabberPCL
{
	public class RestAPI
	{
		// Used to access platform specific implementations
		public static Interfaces.IDiskIO GlobalIO;

		readonly HttpClient _client;

		public RestAPI()
		{
			_client = new HttpClient();
			_client.BaseAddress = new Uri("https://openlab.ncl.ac.uk/dokku/gabber/");
		}

		public async Task<bool> Authenticate(string username, string password)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("username", username),
				new KeyValuePair<string, string>("password", password)
			};
			var response = await _client.PostAsync("api/auth", new FormUrlEncodedContent(pairs));
			return response.IsSuccessStatusCode;
		}

		public async Task<bool> Register(string fullname, string email, string password)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("fullname", fullname),
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};
			var response = await _client.PostAsync("api/register", new FormUrlEncodedContent(pairs));
			return response.IsSuccessStatusCode;
		}

		// As this deals with reading files from platform specific paths, 
		// then we must implement this on each specific platform.
		public async Task<bool> Upload(Story story)
		{
			using (var formData = new MultipartFormDataContent())
			{
				formData.Add(new StringContent(story.InterviewerEmail), "interviewerEmail");
				formData.Add(new StringContent(story.IntervieweeEmail), "intervieweeEmail");
				formData.Add(new StringContent(story.IntervieweeName), "intervieweeName");
				formData.Add(new StringContent(story.Location), "location");
				formData.Add(new StringContent(story.promptText), "promptText");
				// Access the OS specific implementation to load data from a file.
				formData.Add(new ByteArrayContent(GlobalIO.Load(story.AudioPath)),
				             "experience", Path.GetFileName(story.AudioPath));
				
				// Taking a photo is optional
				if (!string.IsNullOrWhiteSpace(story.PhotoPath))
				{
					formData.Add(new ByteArrayContent(GlobalIO.Load(story.PhotoPath)),
					             "authorImage", Path.GetFileName(story.PhotoPath));
				}
				var response = await _client.PostAsync("api/upload", formData);
				return response.IsSuccessStatusCode;
			}
		}

		// Obtains all projects that went through a commissioning process.
		// TODO: make this async
		public List<Project> GetProjects()
		{
			var response = _client.GetAsync("api/projects").Result;
			var contents = response.Content.ReadAsStringAsync().Result;
			return JsonConvert.DeserializeObject<List<Project>>(contents);
		}
	}
}