using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    class DelcomDevice : IOutputDevice
    {
        private const string key = "delcom";
        private const string name = "Delcom";

        string IOutputDevice.Key { get { return key; } }
        string IOutputDevice.Name { get { return name; } }

        public Type DeviceType { get { return typeof(Device); } }

        public string Input { get; set; }

        public XElement ToXml()
        {
            return null;
            //throw new NotImplementedException();
        }

        public void FromXml(XElement element)
        {
            // TODO
            //if (element.Name != key)
            //    throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            var inputs = element.Element("inputs");
            foreach (var input in inputs.Elements()) // TODO: Only handles one input
            {
                Input = input.Name.LocalName;
            }
        }
    }
}