using System.Windows;

namespace ControllerProgramEditor.UI.Dialogs
{
    internal interface IDialogsService
    {
        string SelectController();

        string GetNewProgramName();

        void ShowMessage(string message, string caption, MessageBoxButton button, MessageBoxImage image);
        
        string SelectProgram(IEnumerable<string> programs, Action<string> deleteCallback);

        DataRangeResult SelectDataRange();
    }
}
