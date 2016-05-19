using System;
using System.IO;
using System.Net;
using Serilog;

namespace Emanate.TeamCity
{
    public class TeamCityConnection : ITeamCityConnection
    {
        private readonly Uri baseUri;
        private readonly NetworkCredential networkCredential;
        private readonly bool requiresAuthentication;

        public TeamCityConnection(TeamCityDevice device)
        {
            var rawUri = device.Uri ?? "http://localhost";
            baseUri = new Uri(rawUri);

            requiresAuthentication = device.RequiresAuthentication;
            if (requiresAuthentication)
            {
                var userName = device.UserName;
                var password = device.Password;
                networkCredential = new NetworkCredential(userName, password);
            }
        }

        public string GetProjects()
        {
            Log.Information("=> TeamCityConnection.GetProjects");
            var uri = CreateUri("/httpAuth/app/rest/projects");
            return Request(uri);
        }

        public string GetProject(string projectId)
        {
            Log.Information("=> TeamCityConnection.GetProject({0})", projectId);
            var buildUri = CreateUri($"/httpAuth/app/rest/projects/id:{projectId}");
            return Request(buildUri);
        }

        public string GetBuild(string buildId)
        {
            Log.Information("=> TeamCityConnection.GetBuild({0})", buildId);
            var resultUri = CreateUri($"httpAuth/app/rest/builds?locator=running:all,buildType:(id:{buildId}),count:1");
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
                Log.Information("=> TeamCityConnection.Request({0})", uri);
                var webRequest = CreateWebRequest(uri);
                webRequest.Accept = "application/xml";

                using (var webResponse = webRequest.GetResponse())
                using (var stream = webResponse.GetResponseStream())
                using (var reader = new StreamReader(stream))
                    return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                Log.Error("Team city request failed: " + ex.Message);
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