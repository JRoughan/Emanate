using System.Collections.Generic;
using System.Linq;
using Emanate.Core;
using Emanate.Core.Output;

namespace Emanate.Vso.Admin.Inputs
{
    public partial class VsoInputSelectorView
    {
        private VsoInputSelectorViewModel viewModel;

        public VsoInputSelectorView()
        {
            InitializeComponent();
        }

        protected override async void SetDevice(IDevice device)
        {
            viewModel = new VsoInputSelectorViewModel((VsoDevice)device);
            await viewModel.Initialize();
            DataContext = viewModel;
        }

        public override void SelectInputs(IEnumerable<string> inputs)
        {
            viewModel.SelectInputs(inputs);
        }

        public override IEnumerable<string> GetSelectedInputs()
        {
            return viewModel?.GetSelectedInputs() ?? Enumerable.Empty<string>();
        }
    }
}
