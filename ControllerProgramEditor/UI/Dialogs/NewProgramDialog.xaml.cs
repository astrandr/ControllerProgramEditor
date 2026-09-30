using System.Windows;

namespace ControllerProgramEditor
{
    /// <summary>
    /// Interaction logic for NewProgramDialog.xaml
    /// </summary>
    public partial class NewProgramDialog : Window
    {
        public string ProgramName => NameTextBox.Text;
        public NewProgramDialog()
        {
            InitializeComponent();
        }


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void NameTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OkButton.IsEnabled = !string.IsNullOrWhiteSpace(NameTextBox.Text) && NameTextBox.Text.IndexOf(" ") < 0;
        }
    }
}
