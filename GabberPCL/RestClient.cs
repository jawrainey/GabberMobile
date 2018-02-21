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

	}
}