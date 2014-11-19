﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Emanate.TeamCity.Configuration;

namespace Emanate.TeamCity
{
    public class TeamCityConnection : ITeamCityConnection
    {
        private readonly Uri baseUri;
        private readonly NetworkCredential networkCredential;
        private readonly bool requiresAuthentication;

        public TeamCityConnection(TeamCityConfiguration configuration)
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
            Trace.TraceInformation("=> TeamCityConnection.GetProjects");
            var uri = CreateUri("/httpAuth/app/rest/projects");
            return Request(uri);
        }

        public string GetProject(string projectId)
        {
            Trace.TraceInformation("=> TeamCityConnection.GetProject");
            var buildUri = CreateUri(string.Format("/httpAuth/app/rest/projects/id:{0}", projectId));
            return Request(buildUri);
        }

        public string GetBuild(string buildId)
        {
            Trace.TraceInformation("=> TeamCityConnection.GetBuild");
            var resultUri = CreateUri(string.Format("httpAuth/app/rest/builds?locator=running:all,buildType:(id:{0}),count:1", buildId));
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

            if (requiresAuthentication)
                webRequest.Credentials = networkCredential;

            webRequest.Proxy = null;
            return (webRequest);
        }
    }
}