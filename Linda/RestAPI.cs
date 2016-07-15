using System;
using System.IO;
using System.Net;
using Org.Json;
using RestSharp;

namespace Linda
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

	}
}