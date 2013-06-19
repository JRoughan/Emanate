using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Emanate.Core.Output
{
    public interface IOutputDevice
    {
        string Key { get; }
        string Name { get; }

        Type DeviceType { get; }

        List<Input> Inputs { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }

    public class Input
    {
        public string Source { get; set; }
        public string Id { get; set; }
        public string Profile { get; set; }
    }
}
