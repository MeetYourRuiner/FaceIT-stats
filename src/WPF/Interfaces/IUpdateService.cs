using System;
using System.Threading.Tasks;

namespace FaceitStats.WPF.Interfaces
{
    interface IUpdateService
    {
        Task<bool> CheckForUpdate();
        string CurrentVersion { get; }
        Task UpdateAsync(Action<string> updateProgressSetter);
    }
}
