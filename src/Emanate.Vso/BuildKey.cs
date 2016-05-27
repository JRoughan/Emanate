using System;
using System.Diagnostics;

namespace Emanate.Vso
{
    [DebuggerDisplay("{ProjectId}:{BuildId}")]
    public class BuildKey : IEquatable<BuildKey>
    {
        public BuildKey(Guid projectId, int buildId)
        {
            ProjectId = projectId;
            BuildId = buildId;
        }

        public Guid ProjectId { get; }
        public int BuildId { get; }

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