using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;

namespace Emanate.Vso
{
    public class VsoConnection : IVsoConnection
    {
        private readonly VsoDevice device;
        private readonly Uri baseUri;

        public VsoConnection(VsoDevice device)
        {
            this.device = device;
            var rawUrl = $"https://{this.device.Uri}.visualstudio.com/DefaultCollection/";
            baseUri = new Uri(rawUrl);
        }

        public async Task<ProjectCollection> GetProjects()
        {
            Log.Information("=> VsoConnection.GetProjects");
            return await GetWebResource<ProjectCollection>("_apis/projects?api-version=2");
        }

        // TODO: Change method to GetBuilds as an optimisation for multiple definitions under the same project
        public async Task<Build> GetBuild(Guid projectId, int definition)
        {
            Log.Information("=> VsoConnection.GetBuild({0}, {1})", projectId, definition);
            var buildCollection = await GetWebResource<BuildCollection>($"{projectId}/_apis/build/builds?api-version=2&definitions={definition}&$top=1");
            return buildCollection.Value.Single();
        }

        public async Task<BuildDefinitionCollection> GetBuildDefinitions(Guid projectId)
        {
            Log.Information("=> VsoConnection.GetBuildDefinitions({0})", projectId);
            return await GetWebResource<BuildDefinitionCollection>($"{projectId}/_apis/build/definitions?api-version=2");
        }

        private async Task<TResource> GetWebResource<TResource>(string url)
        {
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, url);
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<TResource>(responseBody);
                }
            }
        }

        private HttpClient CreateHttpClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var authText = System.Text.Encoding.ASCII.GetBytes($"{device.UserName}:{device.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authText));
            return client;
        }
    }
}