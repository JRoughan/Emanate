using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using Emanate.Vso.Configuration;
using Newtonsoft.Json;

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

        public dynamic GetProjects()
        {
            Trace.TraceInformation("=> VsoConnection.GetProjects");
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, "_apis/projects?api-version=2");
                using (var response = client.GetAsync(requestUri).Result)
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
        }

        public dynamic GetBuild(Guid projectId, int definition)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuild({0}, {1})", projectId, definition);
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, $"{projectId}/_apis/build/builds?api-version=2&definitions={definition}&$top=1");
                using (var response = client.GetAsync(requestUri).Result)
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(responseBody);
                }
            }
        }

        public dynamic GetBuildDefinitions(Guid projectId)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuildDefinitions({0})", projectId);
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, $"{projectId}/_apis/build/definitions?api-version=2");
                using (var response = client.GetAsync(requestUri).Result)
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = response.Content.ReadAsStringAsync().Result;
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