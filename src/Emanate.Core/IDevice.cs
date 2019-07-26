using System;
using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IDevice
    {
        Guid Id { get; }
        string Name { get; set; }
        string Key { get; }

        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
    }
}