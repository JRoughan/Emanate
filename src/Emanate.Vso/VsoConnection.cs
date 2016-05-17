using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Emanate.Vso.Configuration;
using Newtonsoft.Json;
using Serilog;

namespace Emanate.Vso
{
    public class VsoConnection : IVsoConnection
    {
        private readonly VsoConfiguration configuration;
        private readonly Uri baseUri;

        public VsoConnection(VsoConfiguration configuration)
        {
            this.configuration = configuration;
            var rawUrl = $"https://{configuration.Uri}.visualstudio.com/DefaultCollection/";
            baseUri = new Uri(rawUrl);
        }

        public async Task<dynamic> GetProjects()
        {
            Log.Information("=> VsoConnection.GetProjects");
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, "_apis/projects?api-version=2");
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
        }

        public async Task<dynamic> GetBuild(Guid projectId, int definition)
        {
            Log.Information("=> VsoConnection.GetBuild({0}, {1})", projectId, definition);
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, $"{projectId}/_apis/build/builds?api-version=2&definitions={definition}&$top=1");
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
        }

        public async Task<dynamic> GetBuildDefinitions(Guid projectId)
        {
            Log.Information("=> VsoConnection.GetBuildDefinitions({0})", projectId);
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, $"{projectId}/_apis/build/definitions?api-version=2");
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var authText = System.Text.Encoding.ASCII.GetBytes($"{configuration.UserName}:{configuration.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authText));
            return client;
        }
    }
}