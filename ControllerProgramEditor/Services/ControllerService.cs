using Microsoft.Extensions.Logging;
using System.Windows;
using Trio.ControllerConnection;

namespace ControllerProgramEditor.Services
{
    internal class ControllerService : IControllerService
    {
        private readonly IControllerConnection controller;
        private readonly ILogger logger;

        public ControllerService(IControllerConnection controller, ILogger logger)
        {
            this.controller = controller;
            this.logger = logger;
        }

        public IEnumerable<string> Programs => controller.IsOpen ? controller.Dir : Enumerable.Empty<string>();

        public async Task ConnectAsync(string controllerName)
        {
             await Task.Run(() => controller.Open(controller.DefaultPeerAddress));
        }

        public void Close()
        {
            if (controller.IsOpen) controller.Close();
        }

        public bool IsOpen => controller.IsOpen;

        public void DeleteProgram(string name)
        {
            controller.DeleteProgram(name);
        }

        public Task UploadProgramAsync(string name, string code, IProgress<int> progress)
        {
            return Task.Run(() => controller.LoadProgramCode(name, code, progress));
        }

        public Task<string> ReadProgramAsync(string name, IProgress<int> progress)
        {

            return Task.Run(() => controller.GetProgramCode(name, progress));
        }

        public Task<int> UploadTableDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            return Task.Run(() => controller.WriteValues(ControllerMemory.TABLE, offSet, values, progress));
        }

        public Task<int> ReadTableDataAsync(int offset, double[] values, IProgress<int> progress)
        {
            return Task.Run( () => controller.ReadValues(ControllerMemory.TABLE, offset, values, progress));
        }

        public Task<int> UploadVRDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            return Task.Run( () => controller.WriteValues(ControllerMemory.VR, offSet, values, progress));
        }

        public Task<int> ReadVRDataAsync(int offSet, double[] values, IProgress<int> progress)
        {
            return Task.Run( () => controller.ReadValues(ControllerMemory.VR, offSet, values, progress));
        }
    }
}
