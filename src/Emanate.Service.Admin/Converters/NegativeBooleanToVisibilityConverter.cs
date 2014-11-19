﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Emanate.Service.Admin.Converters
{
    [Localizability(LocalizationCategory.NeverLocalize)]
    public sealed class NegativeBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = false;
            if (value is bool)
                bValue = (bool)value;

            return (bValue) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility)
                return (Visibility)value != Visibility.Visible;

            return false;
        }
    }
}
