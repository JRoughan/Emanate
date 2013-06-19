using System;
using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public interface IModuleConfiguration
    {
        string Key { get; }
        string Name { get; }

        Type GuiType { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }
}
