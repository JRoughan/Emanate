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

        List<InputInfo> Inputs { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }

    public class InputInfo
    {
        public string Source { get; set; }
        public string Id { get; set; }
        public string Profile { get; set; }
    }
}
