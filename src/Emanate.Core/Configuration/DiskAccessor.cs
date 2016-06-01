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
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            configDoc.Save(path);
        }
    }
}