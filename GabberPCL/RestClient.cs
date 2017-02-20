using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;

namespace GabberPCL
{
	public class RestClient
	{
		// Used to access platform specific implementations
		public static Interfaces.IDiskIO GlobalIO;

		readonly HttpClient _client;

		public RestClient()
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
			return await GottaCatchThemAll("api/auth", new FormUrlEncodedContent(pairs));
		}

		public async Task<bool> Register(string fullname, string email, string password)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("fullname", fullname),
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};
			return await GottaCatchThemAll("api/register", new FormUrlEncodedContent(pairs));
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
				formData.Add(new StringContent(story.IntervieweeGender), "intervieweeGender");
				formData.Add(new StringContent(story.IntervieweeAge), "intervieweeAge");
				formData.Add(new StringContent(story.ComplexNeedsAsJSON), "ComplexNeedsAsJSON");

				// Access the OS specific implementation to load data from a file.
				formData.Add(new ByteArrayContent(GlobalIO.Load(story.AudioPath)),
				             "experience", Path.GetFileName(story.AudioPath));
				
				// Taking a photo is optional
				if (!string.IsNullOrWhiteSpace(story.PhotoPath))
				{
					formData.Add(new ByteArrayContent(GlobalIO.Load(story.PhotoPath)),
					             "authorImage", Path.GetFileName(story.PhotoPath));
				}

				try
				{
					var response = await _client.PostAsync("api/upload", formData);
					return response.IsSuccessStatusCode;
				}
				catch
				{
					return false;
				}
			}
		}

		// Obtains all projects that went through a commissioning process.
		// TODO: make this async
		public List<Project> GetProjects()
		{
			try
			{
				var response = _client.GetAsync("api/projects").Result;
				var contents = response.Content.ReadAsStringAsync().Result;
				return JsonConvert.DeserializeObject<List<Project>>(contents);
			}
			catch
			{
				return new List<Project>();
			}
		}

		// PostAsync has potential for errrors to be thrown, which do not translate well to HTTP Error codes. 
		// We do not care about error handling given we don't present those errors to the user.
		// TODO: we should present users with better error messages based on HTTP codes.
		async Task<bool> GottaCatchThemAll(string path, FormUrlEncodedContent content)
		{
			try
			{
				var response = await _client.PostAsync(path, content);
				return response.IsSuccessStatusCode;
			}
			catch
			{
				return false;
			}
		}
	}
}