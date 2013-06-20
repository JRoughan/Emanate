using System.Windows;

namespace Emanate.Delcom.Configuration
{
    /// <summary>
    /// Interaction logic for AddProfileView.xaml
    /// </summary>
    public partial class AddProfileView : Window
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
