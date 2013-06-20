using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core.Output;

namespace Emanate.Core.Configuration
{
    public interface IModuleConfiguration
    {
        string Key { get; }
        string Name { get; }

        Type GuiType { get; }

        IEnumerable<IOutputProfile> Profiles { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }
}
