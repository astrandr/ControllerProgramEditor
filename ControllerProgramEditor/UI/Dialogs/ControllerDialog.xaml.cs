using System.Windows;

namespace ControllerProgramEditor.UI
{
    public partial class ControllerDialog : Window
    {
        public ControllerDialog()
        {
            InitializeComponent();
        }

        public string ControllerName
        {
            get => ControllerNameTextBox.Text;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Input_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OkButton.IsEnabled = !string.IsNullOrWhiteSpace(ControllerNameTextBox.Text) &&
                                 !ControllerNameTextBox.Text.Any(char.IsWhiteSpace);
        }
    }
}
