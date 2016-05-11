using System;
using System.Collections.Generic;
using System.Diagnostics;
using Emanate.Core.Output;

namespace Emanate.Core.Input
{
    public interface IBuildMonitor
    {
        void BeginMonitoring();
        void EndMonitoring();

        void AddBuilds(IOutputDevice outputDevice, IEnumerable<BuildKey> buildIds);
    }

    [DebuggerDisplay("{ProjectId}:{BuildId}")]
    public class BuildKey
    {
        public BuildKey(Guid projectId, string buildId)
        {
            ProjectId = projectId;
            BuildId = buildId;
        }

        public Guid ProjectId { get; }
        public string BuildId { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals((BuildKey)obj);
        }

        public bool Equals(BuildKey other)
        {
            if (other == null)
                return false;

            return ProjectId.Equals(other.ProjectId) && BuildId.Equals(other.BuildId);
        }

        public override int GetHashCode()
        {
            return (ProjectId.ToString() + BuildId).GetHashCode();
        }
    }
}