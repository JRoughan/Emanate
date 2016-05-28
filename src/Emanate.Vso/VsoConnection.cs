﻿using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, Task<ProjectCollection>> projectCollectionCache = new ConcurrentDictionary<string, Task<ProjectCollection>>();
        private static readonly ConcurrentDictionary<string, Task<BuildDefinitionCollection>> buildDefinitionCollectionCache = new ConcurrentDictionary<string, Task<BuildDefinitionCollection>>();

        private readonly VsoDevice device;
        private readonly Uri baseUri;

        public VsoConnection(VsoDevice device)
        {
            this.device = device;
            var rawUrl = $"https://{this.device.Uri}.visualstudio.com/DefaultCollection/";
            baseUri = new Uri(rawUrl);
        }

        public async Task<ProjectCollection> GetProjects(bool forceRefresh = false)
        {
            Log.Information("=> VsoConnection.GetProjects");
            return await GetCachedWebResource("_apis/projects?api-version=2", projectCollectionCache, forceRefresh);
        }

        // TODO: Change method to GetBuilds as an optimisation for multiple definitions under the same project
        public async Task<Build> GetBuild(Guid projectId, int definition)
        {
            Log.Information("=> VsoConnection.GetBuild({0}, {1})", projectId, definition);
            var buildCollection = await GetWebResource<BuildCollection>($"{projectId}/_apis/build/builds?api-version=2&definitions={definition}&$top=1");
            return buildCollection.Value.Single();
        }

        public async Task<BuildDefinitionCollection> GetBuildDefinitions(Guid projectId, bool forceRefresh = false)
        {
            Log.Information("=> VsoConnection.GetBuildDefinitions({0})", projectId);
            return await GetCachedWebResource($"{projectId}/_apis/build/definitions?api-version=2", buildDefinitionCollectionCache, forceRefresh);
        }

        private async Task<TResource> GetCachedWebResource<TResource>(string url, ConcurrentDictionary<string, Task<TResource>> cache, bool forceRefresh)
        {
            Log.Information("=> VsoConnection.GetCachedWebResource");
            if (forceRefresh)
                return await cache.AddOrUpdate(url, GetWebResource<TResource>, (u, e) => GetWebResource<TResource>(u));

            return await cache.GetOrAdd(url, GetWebResource<TResource>);
        }

        private async Task<TResource> GetWebResource<TResource>(string url)
        {
            using (var client = CreateHttpClient())
            {
                var requestUri = new Uri(baseUri, url);
                Log.Information($"Requesting {typeof(TResource)} from {url}");
                using (var response = await client.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        return JsonConvert.DeserializeObject<TResource>(responseBody);
                    }
                    catch (Exception)
                    {
                        Log.Error($"Cannot deserialise {typeof(TResource)} from JSON {responseBody}");
                        throw;
                    }
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