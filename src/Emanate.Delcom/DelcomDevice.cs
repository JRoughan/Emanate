using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Emanate.Core.Configuration;
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
            Inputs = new List<InputInfo>();
        }

        public Type DeviceType { get { return typeof(Device); } }

        public List<InputInfo> Inputs { get; private set; }

        public string Profile { get; set; }

        public Memento CreateMemento()
        {
            var deviceElement = new XElement("output");
            deviceElement.Add(new XAttribute("type", key));
            deviceElement.Add(new XAttribute("profile", Profile));
            deviceElement.Add(new XElement("name", Name));
            var inputsElement = new XElement("inputs");
            foreach (var input in Inputs)
            {
                var inputElement = new XElement("input");
                inputElement.Add(new XAttribute("source", input.Source));
                inputElement.Add(new XAttribute("id", input.Id));
                inputsElement.Add(inputElement);
            }
            deviceElement.Add(inputsElement);
            return new Memento(deviceElement);
        }

        public void SetMemento(Memento memento)
        {
            if (memento.Type != key)
                throw new ArgumentException("Cannot load non-Delcom device");

            // TODO: Error handling
            var element = memento.Element;
            Profile = element.Attribute("profile").Value;
            name = element.Element("name").Value;
            var inputsElement = element.Element("inputs");
            foreach (var inputElement in inputsElement.Elements("input"))
            {
                var input = new InputInfo();
                input.Source = inputElement.Attribute("source").Value;
                input.Id = inputElement.Attribute("id").Value;
                Inputs.Add(input);
            }
        }
    }
}