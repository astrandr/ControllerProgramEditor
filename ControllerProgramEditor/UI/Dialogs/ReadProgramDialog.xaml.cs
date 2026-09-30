using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace ControllerProgramEditor
{
    public partial class ProgramReadDialog : Window
    {
        public string ProgramName { get; private set; } = "";

        private readonly Action<string> deleteCallback;
        private readonly ObservableCollection<string> programs;

        public ProgramReadDialog(
            IEnumerable<string> programs,
            Action<string> deleteCallback)
        {
            InitializeComponent();

            this.programs = new ObservableCollection<string>(programs);
            ProgramsListBox.ItemsSource = this.programs;
            this.deleteCallback = deleteCallback;
        }

        private void ProgramsListBox_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            bool selected = ProgramsListBox.SelectedItem != null;

            ReadButton.IsEnabled = selected;
            DeleteButton.IsEnabled = selected;
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            ProgramName = ProgramsListBox.SelectedItem!.ToString()!;
            DialogResult = true;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (ProgramsListBox.SelectedItem is string name)
            {
                deleteCallback(name);
                programs.Remove(name);
            }
        }
    }
}