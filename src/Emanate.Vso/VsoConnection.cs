using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Emanate.Vso.Configuration;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;

namespace Emanate.Vso
{
    public class VsoConnection : IVsoConnection
    {
        private readonly Uri baseUri;
        private readonly VssCredentials vssCredential;

        public VsoConnection(VsoConfiguration configuration)
        {
            var rawUrl = $"https://{configuration.Uri}.visualstudio.com/DefaultCollection/";
            baseUri = new Uri(rawUrl);
            var networkCredential = new NetworkCredential(configuration.UserName, configuration.Password);
            var windowsCredential = new WindowsCredential(networkCredential);
            vssCredential = new VssCredentials(windowsCredential);
        }

        public IEnumerable<TeamProjectReference> GetProjects()
        {
            Trace.TraceInformation("=> VsoConnection.GetProjects");
            var client = new ProjectHttpClient(baseUri, vssCredential);
            return client.GetProjects().Result;
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

        public IEnumerable<DefinitionReference> GetBuildDefinitions(Guid projectId)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuilds({0})", projectId);
            var client = new BuildHttpClient(baseUri, vssCredential);
            return client.GetDefinitionsAsync(projectId).Result;
        }
    }
}