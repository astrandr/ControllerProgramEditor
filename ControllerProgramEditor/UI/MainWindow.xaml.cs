using System.Windows;
using Microsoft.Extensions.Logging;

using ControllerProgramEditor.Services;
using ControllerProgramEditor.UI.Dialogs;
using ControllerProgramEditor.UI;

using Trio.ControllerConnection;

namespace ControllerProgramEditor
{
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger logger;

        private readonly IControllerService controllerService;

        private readonly MainViewModel viewModel;

        public MainWindow()
        {
            InitializeComponent();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddDebug();
            });

            logger = loggerFactory.CreateLogger("ControllerProgramEditorLogger");

            var controller = new ControllerDirectConnection();

            controllerService = new ControllerService(controller, logger);

            DataContext = new MainViewModel(controllerService, logger, new DialogsService(this));

        }
    }
}