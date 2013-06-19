using System;
using System.Xml.Linq;

namespace Emanate.Core.Output
{
    public interface IOutputDevice
    {
        string Key { get; }
        string Name { get; }

        Type DeviceType { get; }

        string Input { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }
}
