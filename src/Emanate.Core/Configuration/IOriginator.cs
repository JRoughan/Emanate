using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public interface IOriginator
    {
        Memento CreateMemento();
        void SetMemento(Memento element);
    }

    public class Memento
    {
        public Memento(XElement element)
        {
            Element = element;
            Type = element.Attribute("type").Value;
        }

        public XElement Element { get; private set; }

        public string Type { get; set; }
    }
}