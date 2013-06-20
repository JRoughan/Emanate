using System;
using System.Collections.Generic;
using Emanate.Core.Configuration;
using Emanate.Core.Output;

namespace Emanate.TeamCity.InputSelector
{
    public partial class InputSelectorView
    {
        private readonly InputSelectorViewModel viewModel;

        public InputSelectorView(InputSelectorViewModel inputSelectorViewModel)
        {
            DataContext = viewModel = inputSelectorViewModel;
            Initialized += ViewInitialized;
            InitializeComponent();
        }

        void ViewInitialized(object sender, EventArgs e)
        {
            viewModel.Initialize();
        }

        public override void SelectInputs(IEnumerable<InputInfo> inputs, IModuleConfiguration moduleConfiguration, string currentOutputProfile)
        {
            viewModel.SelectInputs(inputs, moduleConfiguration, currentOutputProfile);
        }
    }
}
