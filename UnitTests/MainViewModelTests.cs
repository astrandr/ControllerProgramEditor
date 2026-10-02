using ControllerProgramEditor.Services;
using ControllerProgramEditor.UI;
using ControllerProgramEditor.UI.Commands;
using ControllerProgramEditor.UI.Dialogs;
using ControllerProgramEditor.UI.ViewModels;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests
{
    public class MainViewModelTests
    {
        private Mock<IControllerService> mockControllerService = null!;
        private Mock<ILogger> mockLogger = null!;
        private Mock<IDialogsService> mockDialogsService = null!;
        private MainViewModel viewModel = null!;

        [SetUp]
        public void Setup()
        {
            mockControllerService = new Mock<IControllerService>();
            mockLogger = new Mock<ILogger>();
            mockDialogsService = new Mock<IDialogsService>();

            viewModel = new MainViewModel(
                mockControllerService.Object,
                mockLogger.Object,
                mockDialogsService.Object
            );
        }

        #region Constructor Tests
        [Test]
        public void Constructor_InitializesProperties()
        {
            // Assert
            Assert.That(viewModel.ProgramName, Is.EqualTo(""));
            Assert.That(viewModel.ProgramText, Is.EqualTo(""));
            Assert.That(viewModel.ProgressValue, Is.EqualTo(0));
            Assert.That(viewModel.IsOperationInProgress, Is.False);
            Assert.That(viewModel.IsConnected, Is.False);
            Assert.That(viewModel.CanConnect, Is.True);
        }

        [Test]
        public void Constructor_InitializesCommands()
        {
            // Assert
            Assert.That(viewModel.NewProgramCommand, Is.Not.Null);
            Assert.That(viewModel.ConnectCommand, Is.Not.Null);
            Assert.That(viewModel.DisconnectCommand, Is.Not.Null);
            Assert.That(viewModel.ReadProgramCommand, Is.Not.Null);
            Assert.That(viewModel.UploadProgramCommand, Is.Not.Null);
            Assert.That(viewModel.ReadDataCommand, Is.Not.Null);
            Assert.That(viewModel.UploadDataCommand, Is.Not.Null);
        }
        #endregion

        #region Property Tests
        [Test]
        public void ProgramName_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ProgramName))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.ProgramName = "TestProgram";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.ProgramName, Is.EqualTo("TestProgram"));
        }

        [Test]
        public void ProgramName_WhenSet_RaisesCanEditPropertyChanged()
        {
            // Arrange
            bool canEditRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.CanEdit))
                    canEditRaised = true;
            };

            // Act
            viewModel.ProgramName = "TestProgram";

            // Assert
            Assert.That(canEditRaised, Is.True);
            Assert.That(viewModel.CanEdit, Is.True);
        }

        [Test]
        public void CanEdit_ReturnsFalseWhenProgramNameEmpty()
        {
            // Assert
            Assert.That(viewModel.CanEdit, Is.False);
        }

        [Test]
        public void CanEdit_ReturnsTrueWhenProgramNameNotEmpty()
        {
            // Arrange
            viewModel.ProgramName = "TestProgram";

            // Assert
            Assert.That(viewModel.CanEdit, Is.True);
        }

        [Test]
        public void ProgramText_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ProgramText))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.ProgramText = "new code";

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.ProgramText, Is.EqualTo("new code"));
        }

        [Test]
        public void IsConnected_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.IsConnected))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.IsConnected = true;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.IsConnected, Is.True);
        }

        [Test]
        public void ConnectionStatus_ReturnsConnectedLabel_WhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.ConnectionStatus, Is.EqualTo("Connected"));
        }

        [Test]
        public void ConnectionStatus_ReturnsNotConnectedLabel_WhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.ConnectionStatus, Is.EqualTo("Not connected"));
        }

        [Test]
        public void ProgressValue_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ProgressValue))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.ProgressValue = 50;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.ProgressValue, Is.EqualTo(50));
        }

        [Test]
        public void IsOperationInProgress_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.IsOperationInProgress))
                    propertyChangedRaised = true;
            };

            // Act
            viewModel.IsOperationInProgress = true;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.IsOperationInProgress, Is.True);
        }

        [Test]
        public void Rows_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            bool propertyChangedRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.Rows))
                    propertyChangedRaised = true;
            };

            var newRows = new List<DataRow> { new DataRow { Index = 0, Value = 42.0 } };

            // Act
            viewModel.Rows = newRows;

            // Assert
            Assert.That(propertyChangedRaised, Is.True);
            Assert.That(viewModel.Rows, Is.EqualTo(newRows));
        }
        #endregion

        #region Command Tests
        [Test]
        public void NewProgramCommand_CanExecute_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.NewProgramCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void NewProgramCommand_CanExecute_ReturnsTrueWhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.NewProgramCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void NewProgramCommand_Execute_CallsDialogService()
        {
            // Arrange
            viewModel.IsConnected = true;
            mockDialogsService.Setup(x => x.GetNewProgramName()).Returns("NewProgram");

            // Act
            viewModel.NewProgramCommand.Execute(null);

            // Assert
            mockDialogsService.Verify(x => x.GetNewProgramName(), Times.Once);
            Assert.That(viewModel.ProgramName, Is.EqualTo("NewProgram"));
        }

        [Test]
        public void DisconnectCommand_CanExecute_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.DisconnectCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void DisconnectCommand_CanExecute_ReturnsTrueWhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.DisconnectCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void DisconnectCommand_Execute_CallsCloseOnService()
        {
            // Act
            viewModel.DisconnectCommand.Execute(null);

            // Assert
            mockControllerService.Verify(x => x.Close(), Times.Once);
        }

        [Test]
        public void CanUploadProgram_ReturnsFalseWhenProgramNameEmpty()
        {
            // Arrange
            viewModel.ProgramText = "code";
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.CanUploadProgram, Is.False);
        }

        [Test]
        public void CanUploadProgram_ReturnsFalseWhenProgramTextEmpty()
        {
            // Arrange
            viewModel.ProgramName = "Program";
            viewModel.IsConnected = true;
            viewModel.ProgramText = "";

            // Assert
            Assert.That(viewModel.CanUploadProgram, Is.False);
        }

        [Test]
        public void CanUploadProgram_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.ProgramName = "Program";
            viewModel.ProgramText = "code";
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.CanUploadProgram, Is.False);
        }

        [Test]
        public void CanUploadProgram_ReturnsTrueWhenAllConditionsMet()
        {
            // Arrange
            viewModel.ProgramName = "Program";
            viewModel.ProgramText = "code";
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.CanUploadProgram, Is.True);
        }

        [Test]
        public void ReadProgramCommand_CanExecute_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.ReadProgramCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void ReadProgramCommand_CanExecute_ReturnsTrueWhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.ReadProgramCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void UploadProgramCommand_CanExecute_ReturnsFalseWhenCannotUpload()
        {
            // Arrange
            viewModel.ProgramName = "";
            viewModel.ProgramText = "";
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.UploadProgramCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void UploadProgramCommand_CanExecute_ReturnsTrueWhenCanUpload()
        {
            // Arrange
            viewModel.ProgramName = "Program";
            viewModel.ProgramText = "code";
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.UploadProgramCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void ReadDataCommand_CanExecute_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.ReadDataCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void ReadDataCommand_CanExecute_ReturnsTrueWhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.ReadDataCommand.CanExecute(null), Is.True);
        }

        [Test]
        public void UploadDataCommand_CanExecute_ReturnsFalseWhenNotConnected()
        {
            // Arrange
            viewModel.IsConnected = false;

            // Assert
            Assert.That(viewModel.UploadDataCommand.CanExecute(null), Is.False);
        }

        [Test]
        public void UploadDataCommand_CanExecute_ReturnsTrueWhenConnected()
        {
            // Arrange
            viewModel.IsConnected = true;

            // Assert
            Assert.That(viewModel.UploadDataCommand.CanExecute(null), Is.True);
        }
        #endregion

        #region IProgress Tests
        [Test]
        public void Report_UpdatesProgressValue()
        {
            // Act
            viewModel.Report(75);

            // Assert
            Assert.That(viewModel.ProgressValue, Is.EqualTo(75));
        }

        [Test]
        public void Report_RaisesProgressValuePropertyChanged()
        {
            // Arrange
            bool progressRaised = false;
            viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(viewModel.ProgressValue))
                    progressRaised = true;
            };

            // Act
            viewModel.Report(50);

            // Assert
            Assert.That(progressRaised, Is.True);
        }
        #endregion

        #region Program Read/Upload Tests
        [Test]
        public async Task UploadProgramCommand_Execute_UploadsCorrectProgramTextToService()
        {
            // Arrange
            const string programName = "TestProgram";
            const string programCode = "BEGIN\n  MOV R1, 100\n  ADD R1, 50\nEND";

            viewModel.ProgramName = programName;
            viewModel.ProgramText = programCode;
            viewModel.IsConnected = true;

            mockControllerService
                .Setup(x => x.UploadProgramAsync(programName, programCode, It.IsAny<IProgress<int>>()))
                .Returns(Task.CompletedTask);

            // Act
            await (viewModel.UploadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            mockControllerService.Verify(
                x => x.UploadProgramAsync(programName, programCode, It.IsAny<IProgress<int>>()),
                Times.Once);
        }

        [Test]
        public async Task ReadProgramCommand_Execute_ReadsAndUpdatesProgramText()
        {
            // Arrange
            const string programName = "LoadedProgram";
            const string programCode = "BEGIN\n  MOV R2, 200\nEND";

            viewModel.IsConnected = true;
            mockDialogsService
                .Setup(x => x.SelectProgram(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<string>>()))
                .Returns(programName);

            mockControllerService
                .Setup(x => x.Programs)
                .Returns(new List<string> { programName });

            mockControllerService
                .Setup(x => x.ReadProgramAsync(programName, It.IsAny<IProgress<int>>()))
                .ReturnsAsync(programCode);

            // Act
            await (viewModel.ReadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            mockControllerService.Verify(
                x => x.ReadProgramAsync(programName, It.IsAny<IProgress<int>>()),
                Times.Once);
            Assert.That(viewModel.ProgramText, Is.EqualTo(programCode));
            Assert.That(viewModel.ProgramName, Is.EqualTo(programName));
        }

        [Test]
        public async Task UploadProgramCommand_Execute_VerifiesCorrectProgramNameAndText()
        {
            // Arrange
            const string programName = "MyProgram";
            const string programCode = "LOOP:\n  JMP LOOP";

            viewModel.ProgramName = programName;
            viewModel.ProgramText = programCode;
            viewModel.IsConnected = true;

            var capturedName = "";
            var capturedCode = "";

            mockControllerService
                .Setup(x => x.UploadProgramAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IProgress<int>>()))
                .Callback<string, string, IProgress<int>>((name, code, progress) =>
                {
                    capturedName = name;
                    capturedCode = code;
                })
                .Returns(Task.CompletedTask);

            // Act
            await (viewModel.UploadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            Assert.That(capturedName, Is.EqualTo(programName));
            Assert.That(capturedCode, Is.EqualTo(programCode));
        }

        [Test]
        public async Task ReadProgramCommand_Execute_VerifiesReadProgramCalledWithCorrectName()
        {
            // Arrange
            const string programName = "TargetProgram";
            const string programCode = "BEGIN\n  NOP\nEND";

            viewModel.IsConnected = true;
            mockDialogsService
                .Setup(x => x.SelectProgram(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<string>>()))
                .Returns(programName);

            mockControllerService
                .Setup(x => x.Programs)
                .Returns(new List<string> { programName });

            var capturedProgramName = "";
            mockControllerService
                .Setup(x => x.ReadProgramAsync(It.IsAny<string>(), It.IsAny<IProgress<int>>()))
                .Callback<string, IProgress<int>>((name, progress) =>
                {
                    capturedProgramName = name;
                })
                .ReturnsAsync(programCode);

            // Act
            await (viewModel.ReadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            Assert.That(capturedProgramName, Is.EqualTo(programName));
            Assert.That(viewModel.ProgramText, Is.EqualTo(programCode));
        }

        [Test]
        public async Task UploadProgramCommand_Execute_PassesProgressToService()
        {
            // Arrange
            viewModel.ProgramName = "TestProg";
            viewModel.ProgramText = "code";
            viewModel.IsConnected = true;

            IProgress<int>? capturedProgress = null;

            mockControllerService
                .Setup(x => x.UploadProgramAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IProgress<int>>()))
                .Callback<string, string, IProgress<int>>((name, code, progress) =>
                {
                    capturedProgress = progress;
                })
                .Returns(Task.CompletedTask);

            // Act
            await (viewModel.UploadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            Assert.That(capturedProgress, Is.Not.Null);
            Assert.That(capturedProgress, Is.SameAs(viewModel));
        }

        [Test]
        public async Task ReadProgramCommand_Execute_PassesProgressToService()
        {
            // Arrange
            const string programName = "TestProg";
            viewModel.IsConnected = true;
            mockDialogsService
                .Setup(x => x.SelectProgram(It.IsAny<IEnumerable<string>>(), It.IsAny<Action<string>>()))
                .Returns(programName);

            mockControllerService
                .Setup(x => x.Programs)
                .Returns(new List<string> { programName });

            IProgress<int>? capturedProgress = null;
            mockControllerService
                .Setup(x => x.ReadProgramAsync(It.IsAny<string>(), It.IsAny<IProgress<int>>()))
                .Callback<string, IProgress<int>>((name, progress) =>
                {
                    capturedProgress = progress;
                })
                .ReturnsAsync("program code");

            // Act
            await (viewModel.ReadProgramCommand as AsyncRelayCommand)!.ExecuteAsync();

            // Assert
            Assert.That(capturedProgress, Is.Not.Null);
            Assert.That(capturedProgress, Is.SameAs(viewModel));
        }
        #endregion
    }
}
