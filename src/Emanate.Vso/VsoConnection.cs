using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Emanate.Vso.Configuration;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;

namespace Emanate.Vso
{
    public class VsoConnection : IVsoConnection
    {
        private readonly VsoConfiguration configuration;
        private readonly Uri baseUri;
        private readonly VssCredentials vssCredential;

        public VsoConnection(VsoConfiguration configuration)
        {
            this.configuration = configuration;
            var rawUrl = $"https://{configuration.Uri}.visualstudio.com/DefaultCollection/";
            baseUri = new Uri(rawUrl);
            var networkCredential = new NetworkCredential(configuration.UserName, configuration.Password);
            var windowsCredential = new WindowsCredential(networkCredential);
            vssCredential = new VssCredentials(windowsCredential);
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

        public TeamProject GetProject(Guid projectId)
        {
            Trace.TraceInformation("=> VsoConnection.GetProject({0})", projectId);
            var client = new ProjectHttpClient(baseUri, vssCredential);
            return client.GetProject(projectId.ToString()).Result;
        }

        public Build GetBuild(Guid projectId, int definition)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuild({0}, {1})", projectId, definition);
            var client = new BuildHttpClient(baseUri, vssCredential);
            var builds = client.GetBuildsAsync(projectId, new[] {definition}).Result;
            return builds.OrderByDescending(b => b.FinishTime).FirstOrDefault();
        }

        public dynamic GetBuildDefinitions(Guid projectId)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuildDefinitions({0})", projectId);
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, projectId + "/_apis/build/definitions?api-version=2");
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