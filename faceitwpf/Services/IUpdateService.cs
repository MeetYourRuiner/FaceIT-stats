using System;
using System.Threading.Tasks;

namespace faceitwpf.Services
{
    interface IUpdateService
    {
        Task<bool> CheckForUpdate();
        string CurrentVersion { get; }
        Task UpdateAsync(Action<string> updateProgressSetter);
    }
}
