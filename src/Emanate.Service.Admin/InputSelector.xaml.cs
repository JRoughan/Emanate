using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Emanate.TeamCity
{
    /// <summary>
    /// Interaction logic for InputSelector.xaml
    /// </summary>
    public partial class InputSelector : UserControl
    {
        private readonly InputSelectorViewModel viewModel;

        public InputSelector()
        {
            DataContext = viewModel = new InputSelectorViewModel();
            Initialized += ViewInitialized;
            InitializeComponent();
        }

        void ViewInitialized(object sender, EventArgs e)
        {
            viewModel.Initialize();
        }
    }
}
