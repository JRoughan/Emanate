using System;
using System.Windows;
using System.Windows.Controls;

namespace Emanate.Service.Admin
{
    public class PropertyTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate IntegerTemplate { get; set; }
        public DataTemplate BooleanTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var propertyType = ((ConfigurationProperty)item).Type;

            if (propertyType == typeof(string))
                return StringTemplate;

            if (propertyType == typeof(int))
                return IntegerTemplate;

            if (propertyType == typeof(bool))
                return BooleanTemplate;

            throw new Exception("Unsupported property type");
        }
    }
}
