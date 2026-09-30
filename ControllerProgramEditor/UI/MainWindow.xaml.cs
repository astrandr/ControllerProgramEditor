using ControllerProgramEditor.Services;
using ControllerProgramEditor.UI;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows;
using Trio.ControllerConnection;

namespace ControllerProgramEditor
{
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged, IProgress<int>
    {
        private readonly ILogger logger;

        private readonly IControllerService controllerService;

        private bool isConnected = false;

        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (isConnected == value)
                    return;

                isConnected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConnectionStatus)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUploadReadData)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanConnect)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUpload)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanConnect)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUploadReadData)));
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

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgramText)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUpload)));
            }
        }

        public string ConnectionStatus => IsConnected ? "Connected" : "Not connected";

        private int tableDataOffset = 0;

        private List<DataRow> rows = [];

        public List<DataRow> Rows
        {
            get => rows;
            set
            {
                if (rows == value)
                    return;

                rows = value;

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rows)));
            }
        }

        private string programName = "";

        public string ProgramName
        {
            get => programName;
            set
            {
                if (programName == value)
                    return;

                programName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgramName)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanEdit)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUpload)));
            }
        }

        public bool CanEdit => !string.IsNullOrEmpty(programName);

        public bool CanUpload => !string.IsNullOrEmpty(programName) && isConnected && !string.IsNullOrEmpty(ProgramText);

        private string controllerName = "";

        public string ControllerName => controllerName;

        public event PropertyChangedEventHandler PropertyChanged;

        private int progressValue = 0;

        public int ProgressValue
        {
            get => progressValue;
            set
            {
                if (progressValue == value)
                    return;

                progressValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ProgressValue)));
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
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOperationInProgress)));
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
            });

            logger = loggerFactory.CreateLogger("MyLogger");
            var controller = new ControllerDirectConnection();
            controllerService = new ControllerService(controller, logger);

        }

        private async void Connect_Click(object sender, RoutedEventArgs e)
        {
            var controllerDlg = new ControllerDialog()
            {
                Owner = this
            };

            if (controllerDlg.ShowDialog() == true)
            {
                try
                {
                    CanConnect = false;
                    await controllerService.ConnectAsync(controllerDlg.ControllerName);
                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    CanConnect = !IsConnected;
                    MessageBox.Show($"Failed to connect to controller. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }


                IsConnected = controllerService.IsOpen;
                CanConnect = !IsConnected;

                controllerName = controllerDlg.ControllerName;
            }
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            controllerService.Close();
            IsConnected = controllerService.IsOpen;
            CanConnect = !IsConnected;
        }

        private void New_Click(object sender, RoutedEventArgs e)
        {
            var newProgramDialog = new NewProgramDialog()
            {
                Owner = this
            };

            if (newProgramDialog.ShowDialog() == true)
            {
                ProgramName = newProgramDialog.ProgramName;
            }
        }

        private async void ReadProgram_Click(object sender, RoutedEventArgs e)
        {
            var programs = controllerService.Programs.ToList();

            var dialog = new ProgramReadDialog(programs, name => controllerService.DeleteProgram(name))
            {
                Owner = this
            };

            if (dialog.ShowDialog() == true)
            {                             
                try
                {
                    ProgramText = await controllerService.ReadProgramAsync(dialog.ProgramName, this);
                    ProgramName = dialog.ProgramName;
                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    CanConnect = !IsConnected;
                    MessageBox.Show($"Failed to read the program. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    logger.LogError("Failed to read the program, {message}", ex.Message);
                }
            }
        }

        private async void UploadProgram_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await controllerService.UploadProgramAsync(programName, ProgramText, this);
                MessageBox.Show("Program upload completed.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                IsConnected = controllerService.IsOpen; CanConnect = !IsConnected;
                MessageBox.Show($"Failed to upload the program.\r\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.LogError("Failed to upload the program, {message}", ex.Message);
            }
        }

        private async void ReadData_Click(object sender, RoutedEventArgs e)
        {
            var dataRangeDlg = new DataRangeDialog()
            {
                Owner = this
            };

            if (dataRangeDlg.ShowDialog() == true)
            {
                var offset = dataRangeDlg.StartIndex;
                var counts = dataRangeDlg.Counts;
                var data = new double[counts];
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
                        MessageBox.Show($"Failed to read data, read count 0.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        logger.LogError("Failed to read data, read count 0.");
                    }
                    
                }
                catch (Exception ex)
                {
                    IsConnected = controllerService.IsOpen;
                    MessageBox.Show($"Failed to read data. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private async void UploadData_Click(object sender, RoutedEventArgs e)
        {
            var values = Rows.Select(r => r.Value).ToArray<double>();
            try
            {
                CanUploadReadData = false;
                IsOperationInProgress = true;
                await controllerService.UploadTableDataAsync(tableDataOffset, values, this);
                MessageBox.Show("Data upload is complete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                IsConnected = controllerService.IsOpen;
                MessageBox.Show($"Failed to upload data. {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                logger.LogError("Failed to upload data, {message}", ex.Message);
            }
            finally
            {
                CanUploadReadData = true;
                IsOperationInProgress = false;
                CanConnect = !IsConnected;
            }
            
        }

        public void Report(int value)
        {
            ProgressValue = value;
        }
    }
}