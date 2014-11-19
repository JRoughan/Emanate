using System;
using System.Windows;

namespace Emanate.Service.Admin.Controls
{
    public partial class TimeRangeSlider
    {
        public TimeRangeSlider()
        {
            Initialized += WireUpInnerSliders;
            InitializeComponent();
        }

        private void WireUpInnerSliders(object sender, EventArgs e)
        {
            LowerSlider.ValueChanged += LowerSlider_ValueChanged;
            UpperSlider.ValueChanged += UpperSlider_ValueChanged;
        }

        private void LowerSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpperSlider.Value = Math.Max(UpperSlider.Value, LowerSlider.Value + 1);

            var upperValue = (uint) UpperSlider.Value;
            var lowerValue = (uint) LowerSlider.Value;

            if (UpperValue > lowerValue)
            {
                LowerValue = lowerValue;
                UpperValue = upperValue;
            }
            else
            {
                UpperValue = upperValue;
                LowerValue = lowerValue;
            }
        }

        private void UpperSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            LowerSlider.Value = Math.Max(Math.Min(UpperSlider.Value - 1, LowerSlider.Value), 0);

            var upperValue = (uint)UpperSlider.Value;
            var lowerValue = (uint)LowerSlider.Value;

            if (UpperValue > lowerValue)
            {
                LowerValue = lowerValue;
                UpperValue = upperValue;
            }
            else
            {
                UpperValue = upperValue;
                LowerValue = lowerValue;
            }
        }

        public uint LowerValue
        {
            get { return (uint)GetValue(LowerValueProperty); }
            set { SetValue(LowerValueProperty, value); }
        }

        public static readonly DependencyProperty LowerValueProperty =
            DependencyProperty.Register("LowerValue", typeof(uint), typeof(TimeRangeSlider), new UIPropertyMetadata(0u, OnLowerValueChanged));

        private static void OnLowerValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (TimeRangeSlider)d;
            slider.LowerSlider.Value = (uint)e.NewValue;
        }

        public uint UpperValue
        {
            get { return (uint)GetValue(UpperValueProperty); }
            set { SetValue(UpperValueProperty, value); }
        }

        public static readonly DependencyProperty UpperValueProperty =
            DependencyProperty.Register("UpperValue", typeof(uint), typeof(TimeRangeSlider), new UIPropertyMetadata(24u, OnUpperValueChanged));

        private static void OnUpperValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var slider = (TimeRangeSlider)d;
            slider.UpperSlider.Value = (uint)e.NewValue;
        }
    }
}
