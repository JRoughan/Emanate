using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Linq;

namespace Emanate.Core.Configuration
{
    public static class XElementExtensions
    {
        public static string GetAttributeString(this XElement element, string attributeName)
        {
            var attribute = element.Attribute(attributeName);
            if (attribute != null)
            {
                Trace.TraceInformation("[{0}]-[{1}]: {2}", element.Name, attributeName, attribute.Value);
                return attribute.Value;
            }

            Trace.TraceWarning("[{0}]-[{1}]: *Missing*", element.Name, attributeName);
            return null;
        }

        public static bool GetAttributeBoolean(this XElement element, string attributeName)
        {
            var rawValue = GetAttributeString(element, attributeName);
            if (rawValue == null)
                return false;

            bool value;
            if (bool.TryParse(rawValue, out value))
                return value;

            Trace.TraceError("[{0}]-[{1}]: *Invalid*", element.Name, attributeName);
            return false;
        }

        public static TEnum GetAttributeEnum<TEnum>(this XElement element, string attributeName, TEnum defaultValue)
            where TEnum : struct
        {
            var rawValue = GetAttributeString(element, attributeName);
            if (rawValue == null)
                return defaultValue;

            TEnum value;
            if (Enum.TryParse(rawValue, out value))
                return value;

            Trace.TraceError("[{0}]-[{1}]: *Invalid*", element.Name, attributeName);
            return defaultValue;
        }

        public static uint GetAttributeUint(this XElement element, string attributeName)
        {
            var rawValue = GetAttributeString(element, attributeName);
            if (rawValue == null)
                return 0;

            uint value;
            if (uint.TryParse(rawValue, out value))
                return value;

            Trace.TraceError("[{0}]-[{1}]: *Invalid*", element.Name, attributeName);
            return 0;
        }

        public static DateTimeOffset GetAttributeDateTime(this XElement element, string attributeName, string exactFormat)
        {
            var rawValue = GetAttributeString(element, attributeName);
            if (rawValue == null)
                return DateTimeOffset.MinValue;

            DateTimeOffset value;
            if (exactFormat == null && DateTimeOffset.TryParse(rawValue, out value))
                return value;
            if (exactFormat != null && DateTimeOffset.TryParseExact(rawValue, exactFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out value))
                return value;

            Trace.TraceError("[{0}]-[{1}]: *Invalid*", element.Name, attributeName);
            return DateTimeOffset.MinValue;
        }
    }
}