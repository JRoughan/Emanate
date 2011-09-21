using System;
using System.Globalization;
using System.IO;
using System.Net;

namespace Emanate.Core.Input.TeamCity
{
    internal class TeamCityConnection
    {
        private readonly Uri baseUri;
        private readonly NetworkCredential networkCredential;
        private readonly bool isGuestAuthentication;

        public TeamCityConnection(IConfiguration configuration)
        {
            var rawUri = configuration.GetString("TeamCityUri");
            baseUri = new Uri(rawUri);

            isGuestAuthentication = configuration.GetBool("TeamCityGuestAuthentication");
            if (!isGuestAuthentication)
            {
                var userName = configuration.GetString("TeamCityUser");
                var password = configuration.GetString("TeamCityPassword");
                networkCredential = new NetworkCredential(userName, password);
            }
        }

        public Uri CreateUri(string relativeUrl)
        {
            return new Uri(baseUri, relativeUrl.TrimStart('/'));
        }

        public string Request(Uri uri)
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