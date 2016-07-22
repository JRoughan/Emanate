using System.IO;
using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public class DiskAccessor : IDiskAccessor
    {
        public XDocument Load(string path)
        {
            if (!File.Exists(path))
                return null;

            return XDocument.Load(path);
        }

        public void Save(XDocument configDoc, string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            configDoc.Save(path);
        }
    }
}