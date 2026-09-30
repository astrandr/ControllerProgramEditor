using System.Windows;

namespace ControllerProgramEditor
{
    /// <summary>
    /// Interaction logic for DataRangeDialog.xaml
    /// </summary>
    public partial class DataRangeDialog : Window
    {
        public int StartIndex => int.TryParse(StartIndexTextBox.Text, out var v) ? v : 0;
        public int Counts => int.TryParse(CountsTextBox.Text, out var v) ? v : 0;

        public DataRangeDialog()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Input_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            OkButton.IsEnabled =
                int.TryParse(StartIndexTextBox.Text, out int startIndex) &&
                int.TryParse(CountsTextBox.Text, out int counts) &&
                startIndex >= 0 &&
                counts > 0;
        }
    }
}
