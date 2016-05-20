using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IDevice
    {
        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
    }
}