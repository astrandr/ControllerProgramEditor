using Microsoft.Extensions.Logging;
using Trio.ControllerConnection;

namespace ControllerProgramEditor.Services
{
    internal class ControllerService : IControllerService
    {
        private IControllerConnection controller;
        private readonly ILogger logger;

        public ControllerService(IControllerConnection controller, ILogger logger)
        {
            this.controller = controller;
            this.logger = logger;
        }

        public IEnumerable<string> Programs => controller.IsOpen ? controller.Dir : new string[] { };

        public async Task<bool> ConnectAsync(string controllerName)
        {
            try
            {
                await Task.Run(() => controller.Open(controller.DefaultPeerAddress));
                return controller.IsOpen;
            }
            catch (Exception ex)
            {
                logger.LogError("Cannot connect to controller {name}, {ex}", controllerName, ex);
                return false;
            }

        }

        public void Close()
        {
            if (controller.IsOpen) controller.Close();
        }

        public bool IsOpen => controller.IsOpen;

        public void CreateProgram(string name)
        {
            if (!controller.IsOpen)
            {
                logger.LogDebug("CreateProgram is called when controller is not connected");
                return;
            }

            controller.CreateProgram(name);
        }

        public void DeleteProgram(string name)
        {
            if (!controller.IsOpen)
            {
                logger.LogDebug("CreateProgram is called when controller is not connected");
                return;
            }

            controller.DeleteProgram(name);
        }

        public Task UploadProgramAsync(string name, string code, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("UploadProgram is called when controller is not connected");
                return Task.CompletedTask;
            }

            return Task.Run(() => controller.LoadProgramCode(name, code, progress));
        }

        public Task<string> ReadProgramAsync(string name, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("ReadProgram is called when controller is not connected");
                return Task.FromResult(string.Empty);
            }
            return Task.Run(() => controller.GetProgramCode(name, progress));
        }

        public Task<int> UploadTableDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("UploadProgram is called when controller is not connected");
                return Task.FromResult(0);
            }

            return Task.Run( () => controller.WriteValues(ControllerMemory.TABLE, offSet, values, progress));
        }

        public Task<int> ReadTableDataAsync(int offset, double[] values, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("ReadTableData is called when controller is not connected");
                return Task.FromResult(0);
            }

            return Task.Run( () => controller.ReadValues(ControllerMemory.TABLE, offset, values, progress));
        }

        public Task<int> UploadVRDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("UploadProgram is called when controller is not connected");
                return Task.FromResult(0);
            }

            return Task.Run( () => controller.WriteValues(ControllerMemory.VR, offSet, values, progress));
        }

        public Task<int> ReadVRDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            if (!controller.IsOpen)
            {
                logger.LogError("ReadTableData is called when controller is not connected");
                return Task.FromResult(0);
            }

            return Task.Run( () => controller.ReadValues(ControllerMemory.VR, offSet, values, progress));
        }
    }
}
