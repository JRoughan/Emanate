using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public class Memento
    {
        public Memento(XElement element)
        {
            Element = element;
            Key = element.GetAttributeString("key");
            Type = element.GetAttributeString("type");
        }

        public XElement Element { get; private set; }
        public string Key { get; private set; }
        public string Type { get; private set; }
    }
}