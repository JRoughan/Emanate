using System;
using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IDevice
    {
        Guid Id { get; }
        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
    }
}