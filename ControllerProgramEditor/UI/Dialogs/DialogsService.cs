using System.Windows;

namespace ControllerProgramEditor.UI.Dialogs
{
    internal class DialogsService : IDialogsService
    {
        private readonly Window owner;

        public DialogsService(Window owner)
        {
            this.owner = owner;
        }

        public string SelectController()
        {
            var controllerDlg = new ControllerDialog()
            {
                Owner = owner
            };

            if (controllerDlg.ShowDialog() == true)
            {
                return controllerDlg.ControllerName;
            }
            else
            {
                return string.Empty;
            }
        }

        public string SelectProgram(IEnumerable<string> programs, Action<string> deleteCallback)
        {
            var dialog = new ProgramReadDialog(programs, deleteCallback)
            {
                Owner = owner
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.ProgramName;
            }
            else return null;
        }

        public void ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image)
        {
            MessageBox.Show(message, caption, button, image);
        }

        public DataRangeResult SelectDataRange()
        {
            var dataRangeDlg = new DataRangeDialog()
            {
                Owner = owner
            };

            if (dataRangeDlg.ShowDialog() == true)
            {
                return new DataRangeResult(true, dataRangeDlg.StartIndex, dataRangeDlg.Counts);
            }
            else
            {
                return new DataRangeResult(false, 0, 0);
            }
        }

        public string GetNewProgramName()
        {
            var newProgramDialog = new NewProgramDialog()
            {
                Owner = owner
            };

            if (newProgramDialog.ShowDialog() == true)
            {
                return newProgramDialog.ProgramName;
            }
            else
            {
                return null;
            }
        }
    }
}
