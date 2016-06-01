using System;
using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IProfile
    {
        Guid Id { get; }
        string Name { get; }

        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
    }
}