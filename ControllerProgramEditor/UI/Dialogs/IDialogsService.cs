using System.Windows;

namespace ControllerProgramEditor.UI.Dialogs
{
    internal interface IDialogsService
    {
        string SelectController();

        string GetNewProgramName();

        void ShowError(string message);

        void ShowInformation(string message);

        string SelectProgram(IEnumerable<string> programs, Action<string> deleteCallback);

        DataRangeResult SelectDataRange();
    }
}
