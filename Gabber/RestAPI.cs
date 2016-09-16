﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Org.Json;
using RestSharp;

namespace Gabber
{
	public class RestAPI
	{
		readonly RestClient _client;

		public RestAPI()
		{
			_client = new RestClient("https://openlab.ncl.ac.uk/dokku/gabber/");
		}

		public void Authenticate(string username, string password, Action<Tuple<bool, string>> callback)
		{
			var request = new RestRequest("api/auth", Method.POST);

			request.AddParameter("username", username);
			request.AddParameter("password", password);

			_client.ExecuteAsync(request, response =>
			{
				if (response.StatusCode == HttpStatusCode.OK)
					callback(new Tuple<bool, string>(true, response.Content));
				else if (response.StatusCode == 0)
					callback(new Tuple<bool, string>(false, "Cannot connect to the internet"));
				else
					callback(new Tuple<bool, string>(false, new JSONObject(response.Content).GetString("error")));
			});
		}

		// TODO: this method is too similar to Authenticate above; abstract logic to a shared method?
		public void Create(string fullname, string email, string password, Action<Tuple<bool, string>> callback)
		{
			var request = new RestRequest("api/register", Method.POST);

			request.AddParameter("fullname", fullname);
			request.AddParameter("email", email);
			request.AddParameter("password", password);

			_client.ExecuteAsync(request, response =>
			{
				if (response.StatusCode == HttpStatusCode.OK)
					callback(new Tuple<bool, string>(true, response.Content));
				else if (response.StatusCode == 0)
					callback(new Tuple<bool, string>(false, "Cannot connect to the internet"));
				else
					callback(new Tuple<bool, string>(false, new JSONObject(response.Content).GetString("error")));
			});
		}

		public void Upload(Story story)
		{
			var request = new RestRequest("api/upload", Method.POST);

			// This is a required file!
			request.AddFile("experience", File.ReadAllBytes(story.AudioPath), 
			                Path.GetFileName(story.AudioPath), "application/octet-stream");

			// Taking a photo of the interviewee is optional.
			if (!string.IsNullOrWhiteSpace(story.PhotoPath))
			{
				request.AddFile("authorImage", File.ReadAllBytes(story.PhotoPath), 
				                Path.GetFileName(story.PhotoPath), "application/octet-stream");
			}

			// These are all required fields
			request.AddParameter("interviewerEmail", story.InterviewerEmail);
			request.AddParameter("intervieweeEmail", story.IntervieweeEmail);
			request.AddParameter("intervieweeName", story.IntervieweeName);
			request.AddParameter("location", story.Location);
			request.AddParameter("promptText", story.promptText);

			_client.ExecuteAsync(request, response =>
			{
				if (response.StatusCode == HttpStatusCode.OK)
				{
					story.Uploaded = true;
					new Model().UpdateStory(story);
				}
			});
		}

		// Obtains all projects that went through a commissioning process.
		public Task<RootObject> GetProjects()
		{
			// TODO: save all this information to a database for cache diff later.
			var request = new RestRequest("api/projects", Method.GET);
			var response = _client.Execute(request);

			// Only return data if all was well.
			if (response.StatusCode == HttpStatusCode.OK)
			{
				return Task.FromResult(JsonConvert.DeserializeObject<RootObject>(response.Content));
			}

			// If there are NO projects associated with that user, then the above would return an empty projects.
			// However, if there's no Internet, let's handle that edge-case. The logic in the view would stay the same
			// since we would check if any projects exist for a given user. Hence, we must return an empty list.
			// TODO: I am sure there is a tidier way to do this, especially the asnc/await aspects...
			return Task.FromResult(new RootObject { projects = new List<Project>() });
		}

	}
}