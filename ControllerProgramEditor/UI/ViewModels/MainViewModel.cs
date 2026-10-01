using ControllerProgramEditor.Services;
using ControllerProgramEditor.UI.Commands;
using ControllerProgramEditor.UI.Dialogs;
using ControllerProgramEditor.UI.ViewModels;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace ControllerProgramEditor.UI
{
    internal class MainViewModel : INotifyPropertyChanged, IProgress<int>
    {
        #region private members
        private readonly IControllerService controllerService;
        private readonly ILogger logger;
        private readonly IDialogsService dlgService;
        private readonly List<IUpdatableCommand> updatableCommands;
        #endregion

        #region properties
        private string controllerName = "";
        public string ControllerName => controllerName;
        private string programName = "";
        public string ProgramName
        {
            get => programName;
            set
            {
                if (programName == value)
                    return;

                programName = value;
                OnPropertyChanged(nameof(ProgramName));
                OnPropertyChanged(nameof(CanEdit));
                OnPropertyChanged(nameof(CanUploadProgram));
            }
        }
        private string programText = "";
        public string ProgramText
        {
            get => programText;
            set
            {
                if (programText == value)
                    return;

                programText = value;

                OnPropertyChanged(nameof(ProgramText));
                OnPropertyChanged(nameof(CanUploadProgram));
                UploadProgramCommand.RaiseCanExecuteChanged();
            }
        }

        private int progressValue = 0;
        public int ProgressValue
        {
            get => progressValue;
            set
            {
                if (progressValue == value)
                    return;

                progressValue = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }
        private List<DataRow> rows = [];
        public List<DataRow> Rows
        {
            get => rows;
            set
            {
                if (rows == value)
                    return;

                rows = value;

                OnPropertyChanged(nameof(Rows));
            }
        }
        private bool isOperationInProgress;
        public bool IsOperationInProgress
        {
            get => isOperationInProgress;
            set
            {
                if (isOperationInProgress == value)
                    return;

                isOperationInProgress = value;
                OnPropertyChanged(nameof(IsOperationInProgress));
            }
        }
        public string ConnectionStatus => IsConnected ? "Connected" : "Not connected";
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void RaiseAllCanExecuteChanged()
        {
            foreach (var cmd in updatableCommands)
            {
                cmd.RaiseCanExecuteChanged();
            }
        }

        #region UI Flags
        private bool isConnected = false;
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected == value)
                    return;

                isConnected = value;
                OnPropertyChanged(nameof(IsConnected));
                OnPropertyChanged(nameof(ConnectionStatus));
                OnPropertyChanged(nameof(CanUploadReadData));
                OnPropertyChanged(nameof(CanConnect));
                OnPropertyChanged(nameof(CanUploadProgram));

                RaiseAllCanExecuteChanged();
            }
        }

        private bool canConnect = true;
        public bool CanConnect
        {
            get => canConnect;
            set
            {
                if (canConnect == value)
                    return;

                canConnect = value;
                OnPropertyChanged(nameof(CanConnect));
            }
        }
        private bool canUploadReadData = true;
        public bool CanUploadReadData
        {
            get => IsConnected && canUploadReadData;
            set
            {
                if (canUploadReadData == value)
                    return;

                canUploadReadData = value;
                OnPropertyChanged(nameof(CanUploadReadData));
            }
        }

        private int tableDataOffset = 0;
        public bool CanEdit => !string.IsNullOrEmpty(programName);
        public bool CanUploadProgram => !string.IsNullOrEmpty(programName) && isConnected && !string.IsNullOrEmpty(ProgramText);
        #endregion

        #region commands
        public RelayCommand NewProgramCommand { get; private set; }
        public AsyncRelayCommand ConnectCommand { get; private set; }
        public RelayCommand DisconnectCommand { get; private set; }
        public AsyncRelayCommand ReadProgramCommand { get; private set; }
        public AsyncRelayCommand UploadProgramCommand { get; private set; }
        public AsyncRelayCommand ReadDataCommand { get; private set; }
        public AsyncRelayCommand UploadDataCommand { get; private set; }
        #endregion

        public MainViewModel(IControllerService controllerService, ILogger logger, IDialogsService dlgService)
        {
            this.controllerService = controllerService;
            this.logger = logger;
            this.dlgService = dlgService;

            this.NewProgramCommand = new RelayCommand(NewProgram, () => true);
            this.ConnectCommand = new AsyncRelayCommand(ConnectAsync, () => CanConnect);
            this.DisconnectCommand = new RelayCommand(Disconnect, () => IsConnected);
            this.ReadProgramCommand = new AsyncRelayCommand(ReadProgramAsync, () => IsConnected);
            this.UploadProgramCommand = new AsyncRelayCommand(UploadProgramAsync, () => CanUploadProgram);
            this.ReadDataCommand = new AsyncRelayCommand(ReadDataAsync, () => CanUploadReadData);
            this.UploadDataCommand = new AsyncRelayCommand(UploadDataAsync, () => CanUploadReadData);

            this.updatableCommands = new List<IUpdatableCommand>
            {
                DisconnectCommand,
                ReadProgramCommand,
                UploadProgramCommand,
                ReadDataCommand,
                UploadDataCommand
            };
        }

        #region command handlers
        private void NewProgram()
        {
            var newProgramName = dlgService.GetNewProgramName();
            if (!string.IsNullOrEmpty(newProgramName))
            {
                ProgramName = newProgramName;
            }
        }

        private async Task ConnectAsync()
        {
            var controllerName = dlgService.SelectController();

            if (!string.IsNullOrEmpty(controllerName))
            {
                try
                {
                    CanConnect = false;
                    await controllerService.ConnectAsync(controllerName);
                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    CanConnect = !IsConnected;
                    dlgService.ShowMessage($"Failed to connect to controller. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                IsConnected = controllerService.IsOpen;
                CanConnect = !IsConnected;

                this.controllerName = controllerName;
            }
        }

        private void Disconnect()
        {
            controllerService.Close();
            IsConnected = controllerService.IsOpen;
            CanConnect = !IsConnected;
        }

        private async Task ReadProgramAsync()
        {
            IEnumerable<string> programs;

            try
            {
                programs = controllerService.Programs.ToList();
            }
            catch (System.Exception ex)
            {
                IsConnected = controllerService.IsOpen;
                CanConnect = !IsConnected;
                dlgService.ShowMessage($"Failed to get list of programs. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.LogError("Failed to get list of programs, {message}", ex.Message);
                return;
            }

            void deleteProgramCallback(string name)
            {
                try
                {
                    controllerService.DeleteProgram(name);
                }
                catch (Exception ex)
                {
                    dlgService.ShowMessage($"Failed to delete the program. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.LogError("Failed to delete the program, {message}", ex.Message);
                    throw;
                }

            }

            var programName = dlgService.SelectProgram(programs, deleteProgramCallback);
            if (!string.IsNullOrEmpty(programName))
            {
                try
                {
                    ProgramText = await controllerService.ReadProgramAsync(programName, this);
                    ProgramName = programName;
                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    CanConnect = !IsConnected;
                    dlgService.ShowMessage($"Failed to read the program. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.LogError("Failed to read the program, {message}", ex.Message);
                }
            }
        }

        private async Task UploadProgramAsync()
        {
            try
            {
                await controllerService.UploadProgramAsync(programName, ProgramText, this);
                dlgService.ShowMessage("Program upload completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                IsConnected = controllerService.IsOpen; CanConnect = !IsConnected;
                dlgService.ShowMessage($"Failed to upload the program.\r\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.LogError("Failed to upload the program, {message}", ex.Message);
            }
        }

        private async Task ReadDataAsync()
        {
            var dataRangeResult = dlgService.SelectDataRange();

            if (dataRangeResult.Accepted)
            {
                var offset = dataRangeResult.Offset;
                var count = dataRangeResult.Count;
                var data = new double[count];
                try
                {
                    CanUploadReadData = false;
                    IsOperationInProgress = true;
                    var readCounts = await controllerService.ReadTableDataAsync(offset, data, this);
                    if (readCounts > 0)
                    {
                        var values = new double[readCounts];
                        Array.Copy(data, values, readCounts);
                        tableDataOffset = offset;
                        Rows = [.. values.Select((value, index) => new DataRow() { Index = index + offset, Value = value })];
                    }
                    else
                    {
                        IsConnected = controllerService.IsOpen;
                        dlgService.ShowMessage($"Failed to read data, read count 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        logger.LogError("Failed to read data, read count 0.");
                    }

                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    dlgService.ShowMessage($"Failed to read data. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.LogError("Failed to read data, {message}", ex.Message);
                }
                finally
                {
                    CanUploadReadData = true;
                    IsOperationInProgress = false;
                    CanConnect = !IsConnected;
                }
            }
        }

        private async Task UploadDataAsync()
        {
            var values = Rows.Select(r => r.Value).ToArray<double>();
            try
            {
                CanUploadReadData = false;
                IsOperationInProgress = true;
                await controllerService.UploadTableDataAsync(tableDataOffset, values, this);
                dlgService.ShowMessage("Data upload is complete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                IsConnected = controllerService.IsOpen;
                dlgService.ShowMessage($"Failed to upload data. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.LogError("Failed to upload data, {message}", ex.Message);
            }
            finally
            {
                CanUploadReadData = true;
                IsOperationInProgress = false;
                CanConnect = !IsConnected;
            }
        }
        #endregion

        //IProgress
        public void Report(int value)
        {
            ProgressValue = value;
        }
    }
}
