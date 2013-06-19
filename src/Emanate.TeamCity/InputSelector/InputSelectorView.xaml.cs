using System;

namespace Emanate.TeamCity.InputSelector
{
    public partial class InputSelectorView
    {
        private readonly InputSelectorViewModel viewModel;

        public InputSelectorView()
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
