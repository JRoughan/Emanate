using System;
using System.Xml.Linq;

namespace Emanate.Core
{
    public interface IProfile
    {
        XElement CreateMemento();
        void SetMemento(XElement deviceElement);
        Guid Id { get; set; }
    }
}