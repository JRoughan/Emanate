using System.Windows;

namespace Emanate.Delcom.Admin.Profiles
{
    public partial class AddProfileView
    {
        public AddProfileView()
        {
            InitializeComponent();
        }

        private void CloseClicked(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
