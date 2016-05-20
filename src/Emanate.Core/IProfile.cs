using System;
using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IProfile
    {
        Guid Id { get; set; }

        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
    }
}