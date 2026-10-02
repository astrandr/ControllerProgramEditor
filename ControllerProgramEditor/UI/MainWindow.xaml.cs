using ControllerProgramEditor.Services;
using ControllerProgramEditor.UI;
using ControllerProgramEditor.UI.Dialogs;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Windows;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            controllerService.Close();

            base.OnClosing(e);
        }
    }

}