using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Windows.Controls;
using System.Xml.Linq;
using Autofac;
using Emanate.Core.Configuration;

namespace Emanate.Service.Admin
{
    public interface IOutputDevice
    {
        string Key { get; }

        XElement ToXml();
        void FromXml(XElement element);
    }

    public class Foo
    {
        public Foo()
        {
            ModuleConfigurations = new List<ConfigurationInfo>();
            OutputDevices = new List<IOutputDevice>();
        }

        public List<ConfigurationInfo> ModuleConfigurations { get; set; }
        public List<IOutputDevice> OutputDevices { get; set; }
    }
}
