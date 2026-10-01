using System;
using System.Windows;
using System.Windows.Controls;

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

        public void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowInformation(string message)
        {
            MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
