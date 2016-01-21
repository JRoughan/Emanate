﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.Vso.Configuration
{
    public class VsoConfiguration : IInputConfiguration
    {
        private const string key = "vso";
        private const string name = "Visual Studio Online";

        string IInputConfiguration.Key { get { return key; } }
        string IInputConfiguration.Name { get { return name; } }

        public string Uri { get; set; }
        public int PollingInterval { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public Memento CreateMemento()
        {
            Trace.TraceInformation("=> VsoConfiguration.CreateMemento");
            var moduleElement = new XElement("module");
            moduleElement.Add(new XAttribute("key", key));
            moduleElement.Add(new XAttribute("type", "input"));
            moduleElement.Add(new XElement("uri", Uri));
            moduleElement.Add(new XElement("polling-interval", PollingInterval));
            moduleElement.Add(new XElement("username", UserName));
            moduleElement.Add(new XElement("password", EncryptDecrypt(Password)));

            return new Memento(moduleElement);
        }

        public void SetMemento(Memento memento)
        {
            Trace.TraceInformation("=> VsoConfiguration.SetMemento");
            if (memento.Key != key)
                throw new ArgumentException("Cannot load non-Visual Studio Online configuration");

            // TODO: Error handling
            var element = memento.Element;
            Uri = element.Element("uri").Value;
            PollingInterval = int.Parse(element.Element("polling-interval").Value);
            UserName = element.Element("username").Value;
            Password = EncryptDecrypt(element.Element("password").Value);
        }

        // TODO: Extrmemely simplistic encrytion used here - will keep honest people honest but not much else
        private static string EncryptDecrypt(string text)
        {
            var outSb = new StringBuilder(text.Length);
            foreach (var c in text)
            {
                var xored = (char)(c ^ 129);
                outSb.Append(xored);
            }
            return outSb.ToString();
        }
    }
}