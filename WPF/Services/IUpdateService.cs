using System;
using System.Threading.Tasks;

namespace FaceitStats.WPF.Services
{
    interface IUpdateService
    {
        Task<bool> CheckForUpdate();
        string CurrentVersion { get; }
        Task UpdateAsync(Action<string> updateProgressSetter);
    }
}
