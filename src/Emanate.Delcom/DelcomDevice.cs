using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core.Output;

namespace Emanate.Delcom
{
    class DelcomDevice : IOutputDevice
    {
        private const string key = "delcom";
        private const string defaultName = "Delcom";

        string IOutputDevice.Key { get { return key; } }

        private string name;
        public string Name { get { return name ?? defaultName; } }

        public DelcomDevice()
        {
            Inputs = new List<Input>();
        }

        public Type DeviceType { get { return typeof(Device); } }

        public List<Input> Inputs { get; private set; }

        public XElement ToXml()
        {
            var deviceElement = new XElement(key);
            deviceElement.Add(new XElement("name", Name));
            var inputsElement = new XElement("inputs");
            foreach (var input in Inputs)
            {
                var inputElement = new XElement("input");
                inputElement.Add(new XAttribute("source", input.Source));
                inputElement.Add(new XAttribute("id", input.Id));
                inputElement.Add(new XAttribute("profile", input.Profile));
                inputsElement.Add(inputElement);
            }
            deviceElement.Add(inputsElement);
            return deviceElement;
        }

        public void FromXml(XElement element)
        {
            // TODO
            //if (element.Name != key)
            //    throw new ArgumentException("Cannot load non-TeamCity configuration");

            // TODO: Error handling
            name = element.Element("name").Value;
            var inputsElement = element.Element("inputs");
            foreach (var inputElement in inputsElement.Elements("input"))
            {
                var input = new Input();
                input.Source = inputElement.Attribute("source").Value;
                input.Id = inputElement.Attribute("id").Value;
                input.Profile = inputElement.Attribute("profile").Value;
                Inputs.Add(input);
            }
        }
    }
}