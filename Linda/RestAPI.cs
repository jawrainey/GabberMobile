using System;
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
	}
}

