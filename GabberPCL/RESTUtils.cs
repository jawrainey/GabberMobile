using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GabberPCL.Models;
using Newtonsoft.Json;

namespace GabberPCL
{
    public static class RESTInterface
    {
        private static HttpClient _client;
        // Exposed this so that UploadSession can access directly.
        public static HttpClient Client
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

        public static async Task<ServerResponse<T>> GET<T>(string route, bool hasAccessToken = true)
        {
            var _response = new ServerResponse<T>
            {
                Data = default(T),
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                if (hasAccessToken)
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token.Access);
                }
                var response = await Client.GetAsync(route);
                var responseContent = await response.Content.ReadAsStringAsync();
               _response = JsonConvert.DeserializeObject<ServerResponse<T>>(responseContent);
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch
            {
                _response.Meta.Messages.Add("GENERAL");
            }

            return _response;
        }

        public static async Task<ServerResponse<T>> POST<T>(string route, object data, bool hasAccessToken = true)
        {
            var _response = new ServerResponse<T>
            {
                Data = default(T),
                Meta = new Meta { Messages = new List<string>(), Success = false }
            };

            try
            {
                if (hasAccessToken)
                {
                    Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token.Access);
                }

                var jsonContent = JsonConvert.SerializeObject(data, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    MaxDepth = 5,
                    Formatting = Formatting.None
                });

                var contentToSend = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await Client.PostAsync(route, contentToSend);
                var responseContent = await response.Content.ReadAsStringAsync();
                _response = JsonConvert.DeserializeObject<ServerResponse<T>>(responseContent);
            }
            catch (HttpRequestException)
            {
                _response.Meta.Messages.Add("NO_INTERNET");
            }
            catch
            {
                _response.Meta.Messages.Add("GENERAL");
            }

            return _response;
        }
    }
}
