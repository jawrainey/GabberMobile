using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using GabberPCL.Models;

namespace GabberPCL
{
    class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }

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

        public async Task<JWToken> Login(string email, string password, Action<string> errorCallback)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};

            try
            {
                var response = await _client.PostAsync("api/auth/login/", new FormUrlEncodedContent(pairs));
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<JWToken>(content);
                }
                errorCallback(JsonConvert.DeserializeObject<Error>(content).Message);
            }
            catch (HttpRequestException)
            {
                errorCallback("You are not connected to the Internet");
            }
            catch (Exception e)
            {
                errorCallback("An unknown error occurred:" + e.Message); 
            }
            return new JWToken();
        }

        public async Task<JWToken> Register(string fullname, string email, string password, Action<string> errorCallback)
		{
			var pairs = new List<KeyValuePair<string, string>>
			{
				new KeyValuePair<string, string>("fullname", fullname),
				new KeyValuePair<string, string>("email", email),
				new KeyValuePair<string, string>("password", password)
			};

            try
            {
                var response = await _client.PostAsync("api/auth/register/", new FormUrlEncodedContent(pairs));
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<JWToken>(content);
                }
                errorCallback(JsonConvert.DeserializeObject<Error>(content).Message);
            }
            catch (HttpRequestException)
            {
                errorCallback("You are not connected to the Internet");
            }
            catch (Exception e)
            {
                errorCallback("An unknown error occurred:" + e.Message);
            }
            return new JWToken();
		}

		// As this deals with reading files from platform specific paths, 
		// then we must implement this on each specific platform.
        public async Task<bool> Upload(InterviewSession interviewSession)
		{
			using (var formData = new MultipartFormDataContent())
			{
                formData.Add(new StringContent(interviewSession.CreatorEmail), "creatorEmail");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Participants)), "participants");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Prompts)), "prompts");
				// Access the OS specific implementation to load data from a file.
                formData.Add(new ByteArrayContent(GlobalIO.Load(interviewSession.RecordingURL)), "recording", 
                             Path.GetFileName(interviewSession.RecordingURL));
				try
				{
                    var endpoint = "api/projects/" + interviewSession.ProjectID.ToString() + "/sessions/";
                    _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Session.Token.Access);
                    var response = await _client.PostAsync(endpoint, formData);
					return response.IsSuccessStatusCode;
				}
				catch
				{
					return false;
				}
			}
		}

        public async Task<List<Project>> GetProjects(Action<string> errorCallback)
		{
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + Session.Token.Access);   

            try
            {
                var response = await _client.GetAsync("api/projects/");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                
                    // Projects are sent as a dictionary of 'private' projects and 'public where
                    // private means if they are a member; public projects do not contain duplicates from private. 
                    var res = JsonConvert.DeserializeObject<Dictionary<string, List<Project>>>(content);
                    var projects = new List<Project>();
                    projects.AddRange(res["personal"]);
                    projects.AddRange(res["public"]);
                    return projects;
                }
                errorCallback(JsonConvert.DeserializeObject<Error>(content).Message);
            }
            catch (HttpRequestException)
            {
                errorCallback("You are not connected to the Internet");
            }
            catch (Exception e)
            {
                errorCallback("An unknown error occurred:" + e.Message);
            }

            return new List<Project>();
		}

	}
}