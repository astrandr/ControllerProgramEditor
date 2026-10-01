using System.Windows.Input;

namespace ControllerProgramEditor.UI.Commands
{
    internal class AsyncRelayCommand : ICommand, IUpdatableCommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

        public AsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public async void Execute(object parameter)
        {
            await execute();
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke() ?? true;
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler CanExecuteChanged;
    }
}
