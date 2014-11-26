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
            StartSlider.ValueChanged += UpdateEndValue;
            EndSlider.ValueChanged += UpdateStartValue;
        }

        private void UpdateEndValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            EndSlider.Value = Math.Max(EndSlider.Value, StartSlider.Value + 1);
            ConstrainValues();
        }

        private void UpdateStartValue(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            StartSlider.Value = Math.Max(Math.Min(EndSlider.Value - 1, StartSlider.Value), 0);
            ConstrainValues();
        }

        public static readonly DependencyProperty StartProperty = DependencyProperty.Register("Start", typeof(uint), typeof(TimeRangeSlider), new FrameworkPropertyMetadata(0u, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public uint Start
        {
            get { return (uint)GetValue(StartProperty); }
            set { SetValue(StartProperty, value); }
        }

        public static readonly DependencyProperty EndProperty = DependencyProperty.Register("End", typeof(uint), typeof(TimeRangeSlider), new FrameworkPropertyMetadata(24u, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public uint End
        {
            get { return (uint)GetValue(EndProperty); }
            set { SetValue(EndProperty, value); }
        }

        private void ConstrainValues()
        {
            var upperValue = (uint)EndSlider.Value;
            var lowerValue = (uint)StartSlider.Value;

            if (upperValue > lowerValue)
            {
                Start = lowerValue;
                End = upperValue;
            }
            else
            {
                End = upperValue;
                Start = lowerValue;
            }
        }
    }
}
