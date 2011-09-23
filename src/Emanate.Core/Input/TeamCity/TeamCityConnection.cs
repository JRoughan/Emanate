using System;
using System.IO;
using System.Net;

namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityConnection : ITeamCityConnection
    {
        private readonly Uri baseUri;
        private readonly NetworkCredential networkCredential;
        private readonly bool isGuestAuthentication;

        public TeamCityConnection(TeamCityConfiguration configuration)
        {
            var rawUri = configuration.Uri ?? "http://localhost";
            baseUri = new Uri(rawUri);

            isGuestAuthentication = configuration.IsUsingGuestAuthentication;
            if (!isGuestAuthentication)
            {
                var userName = configuration.UserName;
                var password = configuration.Password;
                networkCredential = new NetworkCredential(userName, password);
            }
        }

        public string GetProjects()
        {
            var uri = CreateUri("/httpAuth/app/rest/projects");
            return Request(uri);
        }

        public string GetProject(string projectId)
        {
            var buildUri = CreateUri(string.Format("/httpAuth/app/rest/projects/id:{0}", projectId));
            return Request(buildUri);
        }

        public string GetRunningBuilds()
        {
            var runningUri = CreateUri("httpAuth/app/rest/builds?locator=running:true");
            return Request(runningUri);
        }

        public string GetBuild(string buildId)
        {
            var resultUri = CreateUri(string.Format("httpAuth/app/rest/buildTypes/id:{0}/builds", buildId));
            return Request(resultUri);
        }

        private Uri CreateUri(string relativeUrl)
        {
            return new Uri(baseUri, relativeUrl.TrimStart('/'));
        }

        private string Request(Uri uri)
        {
            var webRequest = CreateWebRequest(uri);
            webRequest.Accept = "application/xml";

            using (var webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        private HttpWebRequest CreateWebRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);

            if (!isGuestAuthentication)
                webRequest.Credentials = networkCredential;

            webRequest.Proxy = null;
            return (webRequest);
        }
    }
}