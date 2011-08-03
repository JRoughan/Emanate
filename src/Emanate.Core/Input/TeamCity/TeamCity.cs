using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace Emanate.Core.Input.TeamCity
{
    public class TeamCityMonitor
    {
        private readonly TeamCityConnection connection = new TeamCityConnection("xxx", false);

        public string GetProjects()
        {
            var uri = connection.CreateUri("/httpAuth/app/rest/projects");

            return connection.Request(uri);
        }
    }

    public class TeamCityConnection
    {
        private readonly Uri baseUri;

        public TeamCityConnection(string hostName, bool isSsl)
        {
            var protocol = isSsl ? "https" : "http";
            baseUri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}://{1}", protocol, hostName));
        }

        public Uri CreateUri(string relativeUrl)
        {
            if (relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Substring(1);

            return new Uri(baseUri, relativeUrl);
        }

        public string Request(Uri uri)
        {
            var webRequest = CreateWebRequest(uri);
            webRequest.Accept = "application/xml";

            using (WebResponse webResponse = webRequest.GetResponse())
            using (var stream = webResponse.GetResponseStream())
            using (var reader = new StreamReader(stream))
                return reader.ReadToEnd();
        }

        public HttpWebRequest CreateWebRequest(Uri uri)
        {
            var webRequest = (HttpWebRequest)WebRequest.Create(uri);
            // HACK: Remove hardcoding
            webRequest.Credentials = new NetworkCredential("xxx", "xxx");
            webRequest.Proxy = null;
            return (webRequest);
        }
    }
}
