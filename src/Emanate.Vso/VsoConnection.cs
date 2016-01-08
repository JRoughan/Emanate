using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Emanate.Vso.Configuration;

namespace Emanate.Vso
{
    public class VsoConnection : IVsoConnection
    {
        private readonly Uri baseUri;
        private readonly NetworkCredential networkCredential;
        private readonly bool requiresAuthentication;

        public VsoConnection(VsoConfiguration configuration)
        {
            var rawUri = configuration.Uri ?? "http://localhost";
            baseUri = new Uri(rawUri);

            requiresAuthentication = configuration.RequiresAuthentication;
            if (requiresAuthentication)
            {
                var userName = configuration.UserName;
                var password = configuration.Password;
                networkCredential = new NetworkCredential(userName, password);
            }
        }

        public string GetProjects()
        {
            Trace.TraceInformation("=> VsoConnection.GetProjects");
            var uri = CreateUri("/httpAuth/app/rest/projects");
            return Request(uri);
        }

        public string GetProject(string projectId)
        {
            Trace.TraceInformation("=> VsoConnection.GetProject({0})", projectId);
            var buildUri = CreateUri(string.Format("/httpAuth/app/rest/projects/id:{0}", projectId));
            return Request(buildUri);
        }

        public string GetBuild(string buildId)
        {
            Trace.TraceInformation("=> VsoConnection.GetBuild({0})", buildId);
            var resultUri = CreateUri(string.Format("httpAuth/app/rest/builds?locator=running:all,buildType:(id:{0}),count:1", buildId));
            return Request(resultUri);
        }

        private Uri CreateUri(string relativeUrl)
        {
            return new Uri(baseUri, relativeUrl.TrimStart('/'));
        }

        private string Request(Uri uri)
        {
            try
            {
                Trace.TraceInformation("=> VsoConnection.Request({0})", uri);
                var webRequest = CreateWebRequest(uri);
                webRequest.Accept = "application/xml";

                using (var webResponse = webRequest.GetResponse())
                using (var stream = webResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Team city request failed: " + ex.Message);
                throw;
            }
        }

        private HttpWebRequest CreateWebRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);

            if (requiresAuthentication)
                webRequest.Credentials = networkCredential;

            webRequest.Proxy = null;
            return (webRequest);
        }
    }
}