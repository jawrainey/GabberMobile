using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using GabberPCL.Models;

namespace GabberPCL
{
	public class RestClient
	{
		// Used to access platform specific implementations
		public static Interfaces.IDiskIO GlobalIO;

		readonly HttpClient _client;

		public RestClient()
		{
            _client = new HttpClient
            {
                BaseAddress = new Uri("https://gabber.audio")
            };
        }

        public async Task<JWToken> Login(string email, string password)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};
            // TODO: propagate exception/error message upwards?
            var response = await _client.PostAsync("api/auth/login/", new FormUrlEncodedContent(pairs));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JWToken>(content);
            }
            return new JWToken();
        }

        public async Task<JWToken> Register(string fullname, string email, string password)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("fullname", fullname),
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};

            var response = await _client.PostAsync("api/auth/register/", new FormUrlEncodedContent(pairs));
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return JsonConvert.DeserializeObject<JWToken>(content);
            }
            return new JWToken();
		}

		// As this deals with reading files from platform specific paths, 
		// then we must implement this on each specific platform.
        public async Task<bool> Upload(InterviewSession interviewSession)
		{
			using (var formData = new MultipartFormDataContent())
			{
                formData.Add(new StringContent(interviewSession.ProjectID.ToString()), "projectID");
                formData.Add(new StringContent(interviewSession.CreatorID.ToString()), "creatorID");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Participants)), "participants");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Prompts)), "prompts");
				// Access the OS specific implementation to load data from a file.
                formData.Add(new ByteArrayContent(GlobalIO.Load(interviewSession.RecordingURL)), "recording", 
                             Path.GetFileName(interviewSession.RecordingURL));

				try
				{
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Session.Token.Access);
                    var response = await _client.PostAsync("api/interview/", formData);
					return response.IsSuccessStatusCode;
				}
				catch
				{
					return false;
				}
			}
		}

        // TODO: all projects are sent over, whether public or private, as one list
		public async Task<List<Project>> GetProjects()
		{
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Session.Token.Access);                   
            var response = await _client.GetStringAsync("api/projects/");
            return JsonConvert.DeserializeObject<List<Project>>(response);
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