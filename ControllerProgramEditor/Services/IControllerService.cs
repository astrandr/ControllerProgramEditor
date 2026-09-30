
namespace ControllerProgramEditor.Services
{
    internal interface IControllerService
    {
        IEnumerable<string> Programs { get; }

        Task<bool> ConnectAsync(string controllerName);

        void Close();
        
        bool IsOpen { get; }
        void CreateProgram(string name);

        void DeleteProgram(string name);
        Task<string> ReadProgramAsync(string name, IProgress<int> progress);
        Task<int> ReadTableDataAsync(int offSet, double[] values, IProgress<int> progress);
        Task<int> ReadVRDataAsync(int offset, double[] values, IProgress<int> progress);
        Task UploadProgramAsync(string name, string code, IProgress<int> progress);
        Task<int> UploadTableDataAsync(int offSet, double[] values, IProgress<int> progress);
        Task<int> UploadVRDataAsync(int offSet, double[] values, IProgress<int> progress);
    }
}