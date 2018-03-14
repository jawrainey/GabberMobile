using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using GabberPCL.Models;

namespace GabberPCL
{
    // The Upload method does not return a custom response.
    class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
    // The Data attribute of a response can be of any topic
    class Entity<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    class DataUserTokens
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("tokens")]
        public JWToken Tokens { get; set; }
    }

    class CustomAuthResponse : Entity<DataUserTokens>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    class CustomProjectsResponse : Entity<Dictionary<string, List<Project>>>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    class CustomErrorResponse : Entity<List<string>>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    class Meta
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("messages")]
        public List<string> Messages { get; set; }
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
                BaseAddress = new Uri("https://81dbce0c.ngrok.io")
            };
        }

        public async Task<JWToken> Login(string email, string password, Action<string> errorCallback)
		{
            try
            {
                var payload = new { email, password };
                var _content = new StringContent(JsonConvert.SerializeObject(payload), System.Text.Encoding.UTF8, "application/json");
                var response = await _client.PostAsync("api/auth/login/", _content);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<CustomAuthResponse>(content).Data.Tokens;
                }
                // TODO: use error code to lookup string that has the associated error message.
                errorCallback(JsonConvert.DeserializeObject<CustomErrorResponse>(content).Meta.Messages[0]);
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
            try
            {
                var payload = JsonConvert.SerializeObject(new { fullname, email, password });
                var _content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

                var response = await _client.PostAsync("api/auth/register/", _content);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<CustomAuthResponse>(content).Data.Tokens;
                }
                errorCallback(JsonConvert.DeserializeObject<CustomErrorResponse>(content).Meta.Messages[0]);
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
                    var res = JsonConvert.DeserializeObject<CustomProjectsResponse>(content).Data;
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