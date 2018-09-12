using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.IO;
using GabberPCL.Models;
using System.Text;
using System.Net.Http.Headers;

namespace GabberPCL
{
    // The Upload method does not return a custom response.
    class Error
    {
        [JsonProperty("message")]
        public string Message { get; set; }
    }
    // The Data attribute of a response can be of any topic
    public class Entity<T>
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class DataUserTokens
    {
        [JsonProperty("user")]
        public User User { get; set; }
        [JsonProperty("tokens")]
        public JWToken Tokens { get; set; }
    }

    public class CustomAuthResponse : Entity<DataUserTokens>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class RegisterAuthResponse
    {
        [JsonProperty("data")]
        public bool? Data;
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class RegisterVerifyAuthResponse<T>
    {
        [JsonProperty("data")]
        public T Data;
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class CustomProjectsResponse : Entity<List<Project>>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class CustomLanguagesResponse
    {
        public Meta Meta { get; set; }
        public List<LanguageChoice> Data { get; set; }
    }

    public class CustomErrorResponse : Entity<List<string>>
    {
        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }

    public class Meta
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("messages")]
        public List<string> Messages { get; set; }
    }

    public static class RestClient
    {
        // Used to access platform specific implementations
        public static Interfaces.IDiskIO GlobalIO;

        private static HttpClient _client;

        private static HttpClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new HttpClient
                    {
                        BaseAddress = new Uri(Config.API_ENDPOINT)
                    };
                }

                return _client;
            }
        }

        public static async Task<CustomAuthResponse> Login(string email, string password)
        {
            var _response = new CustomAuthResponse
            {
                Data = null,
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                var payload = new { email, password };
                var _content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                var response = await Client.PostAsync("api/auth/login/", _content);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<CustomAuthResponse>(content);
                }
                _response.Meta = JsonConvert.DeserializeObject<CustomErrorResponse>(content).Meta;
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch (Exception)
            {
                _response.Meta.Messages.Add("GENERAL");
            }
            return _response;
        }

        public static async Task<CustomAuthResponse> Register(string fullname, string email, string password, int lang, int society, Gender genderObj, int role, int age)
        {
            var _response = new CustomAuthResponse
            {
                Data = null,
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                int gender = genderObj.GetId();
                string custom = genderObj.Data;

                var payload = JsonConvert.SerializeObject(new { fullname, email, password, lang, society, gender, custom, role, age });
                var _content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await Client.PostAsync("api/auth/register/", _content);
                var content = await response.Content.ReadAsStringAsync();

                _response.Meta = JsonConvert.DeserializeObject<RegisterAuthResponse>(content).Meta;
                return _response;
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch (Exception)
            {
                _response.Meta.Messages.Add("GENERAL");
            }
            return _response;
        }

        public static async Task<CustomAuthResponse> PushUpdateForCurrentUser()
        {
            var _response = new CustomAuthResponse
            {
                Data = null,
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                var payload = JsonConvert.SerializeObject(Session.ActiveUser);
                var _content = new StringContent(payload, Encoding.UTF8, "application/json");

                var response = await Client.PutAsync("api/auth/me/", _content);
                var content = await response.Content.ReadAsStringAsync();

                _response.Meta = JsonConvert.DeserializeObject<RegisterAuthResponse>(content).Meta;
                return _response;
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch (Exception)
            {
                _response.Meta.Messages.Add("GENERAL");
            }
            return _response;
        }

        public static async Task<RegisterVerifyAuthResponse<DataUserTokens>> RegisterVerify(string token)
        {
            var _response = new RegisterVerifyAuthResponse<DataUserTokens>
            {
                Data = null,
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                var _content = new StringContent("", Encoding.UTF8, "application/json");
                var response = await Client.PostAsync($"api/auth/verify/{token}/", _content);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<RegisterVerifyAuthResponse<DataUserTokens>>(content);
                }

                // If the request cannot be made, this will try to be serialzied and throw an error too.
                _response.Meta = JsonConvert.DeserializeObject<CustomErrorResponse>(content).Meta;
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch (Exception)
            {
                _response.Meta.Messages.Add("GENERAL");
            }
            return _response;
        }

        // As this deals with reading files from platform specific paths, 
        // then we must implement this on each specific platform.
        public static async Task<bool> Upload(InterviewSession interviewSession)
        {
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Participants)), "participants");
                formData.Add(new StringContent(JsonConvert.SerializeObject(interviewSession.Prompts)), "prompts");
                formData.Add(new StringContent(interviewSession.ConsentType.ToString()), "consent");
                formData.Add(new StringContent(interviewSession.Lang.ToString()), "lang");
                formData.Add(new StringContent(interviewSession.CreatedAt.ToString(System.Globalization.CultureInfo.InvariantCulture)), "created_on");

                // Access the OS specific implementation to load data from a file.
                formData.Add(new ByteArrayContent(GlobalIO.Load(interviewSession.RecordingURL)), "recording",
                             Path.GetFileName(interviewSession.RecordingURL));
                try
                {
                    var endpoint = $"api/projects/{interviewSession.ProjectID.ToString()}/sessions/";
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token.Access);
                    var response = await Client.PostAsync(endpoint, formData);
                    return response.IsSuccessStatusCode;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public static async Task<List<Project>> GetProjects(Action<string> errorCallback)
        {
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token.Access);

            try
            {
                var response = await Client.GetAsync("api/projects/");
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<CustomProjectsResponse>(content).Data;
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

        // TODO
        public static async Task<List<IFRC_Society>> GetSocieties(Action<string> errorCallback)
        {
            return new List<IFRC_Society>()
            {
                new IFRC_Society {
                    Id = 0,
                    Name = "Self Preservation Society"
                },
                new IFRC_Society {
                    Id = 1,
                    Name = "Newcastle University Rock Soc"
                },

            };
        }

        public static async Task<List<LanguageChoice>> GetLanguages(Action<string> errorCallback)
        {
            try
            {
                HttpResponseMessage response = await Client.GetAsync("api/help/languages/");
                string content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<CustomLanguagesResponse>(content).Data;
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

            return new List<LanguageChoice>();
        }

    }
}