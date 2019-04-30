using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using GabberPCL.Models;
using System.Net.Http.Headers;

namespace GabberPCL
{
    public static class RestClient
    {
        // Used to access platform specific implementations
        public static Interfaces.IDiskIO GlobalIO;

        public static async Task<ServerResponse<DataUserTokens>> Login(string email, string password)
        {
            var LOGIN_ROUTE = "api/auth/login/";
            return (await RESTInterface.POST<DataUserTokens>(LOGIN_ROUTE, new { email, password }, false));
        }

        public static async Task<ServerResponse<DataUserTokens>> Register(string fullname, string email, string password, int lang)
        {
            var REGISTER_ROUTE = "api/auth/register/";
            var payload = new { fullname, email, password, lang };
            return (await RESTInterface.POST<DataUserTokens>(REGISTER_ROUTE, payload, false));
        }

        public static async Task<ServerResponse<DataUserTokens>> PushUpdateForCurrentUser()
        {
            var ME_ROUTE = "api/auth/me/";
            return (await RESTInterface.POST<DataUserTokens>(ME_ROUTE, Session.ActiveUser));
        }

        public static async Task<ServerResponse<DataUserTokens>> RegisterVerify(string token)
        {
            var REGISTER_VERIFY = $"api/auth/verify/{token}/";
            return (await RESTInterface.POST<DataUserTokens>(REGISTER_VERIFY, Session.ActiveUser, false));
        }

        public static async Task<List<Project>> GetProjects(Action<string> errorCallback)
        {
            var PROJECTS_ROUTE = "api/projects/";
            var response = (await RESTInterface.GET<List<Project>>(PROJECTS_ROUTE));
            if (response.Meta.Messages.Count > 0) errorCallback("PROJECTS ERROR");
            return response.Data;
        }

        public static async Task<List<LanguageChoice>> GetLanguages()
        {
            var LANGUAGES_ROUTE = "api/help/languages/";
            var response = (await RESTInterface.GET<List<LanguageChoice>> (LANGUAGES_ROUTE, false));
            return response.Data;
        }

        public static async Task<bool> UploadFCMToken(string token)
        {
            var response = (await RESTInterface.POST<string>("api/fcm/", new { token }, true));
            return response.Meta.Messages.Count > 0;
        }

        public static async Task<bool> Upload(InterviewSession interviewSession)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Participants)), "participants");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Prompts)), "prompts");
                formData.Add(new StringContent(interviewSession.ConsentType), "consent");
                formData.Add(new StringContent(interviewSession.Lang.ToString()), "lang");
                formData.Add(new StringContent(interviewSession.CreatedAt.ToString(System.Globalization.CultureInfo.InvariantCulture)), "created_on");

                // Access the OS specific implementation to load data from a file.
                formData.Add(new ByteArrayContent(GlobalIO.Load(interviewSession.RecordingURL)), "recording",
                             Path.GetFileName(interviewSession.RecordingURL));

                try
                {
                    // We must use the 'old' POST approach as our util sends JSON. To access the client we must expose it.
                    var endpoint = $"api/projects/{interviewSession.ProjectID.ToString()}/sessions/";
                    RESTInterface.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token.Access);
                    var response = await RESTInterface.Client.PostAsync(endpoint, formData);
                    return response.IsSuccessStatusCode;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}