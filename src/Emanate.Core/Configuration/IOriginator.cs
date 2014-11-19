using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public interface IOriginator
    {
        Memento CreateMemento();
        void SetMemento(Memento memento);
    }

    public class Memento
    {
        public Memento(XElement element)
        {
            Element = element;
            Type = element.GetAttributeString("type");
        }

        public XElement Element { get; private set; }
        public string Type { get; private set; }
    }
}