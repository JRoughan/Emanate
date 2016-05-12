using System;
using System.Collections.Generic;
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

        async void ViewInitialized(object sender, EventArgs e)
        {
            await viewModel.Initialize();
        }

        public override void SelectInputs(IEnumerable<InputInfo> inputs)
        {
            viewModel.SelectInputs(inputs);
        }

        public override IEnumerable<InputInfo> GetSelectedInputs()
        {
            return viewModel.GetSelectedInputs();
        }
    }
}
