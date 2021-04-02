using FaceitStats.WPF.Services;
using System;

namespace FaceitStats.WPF.Interfaces
{
    interface INotifyService
    {
        event EventHandler<NotificationCreatedEventArgs> NotificationCreated;
        event EventHandler NotificationRemoved;

        void DisplayError(Exception exception);
    }
}
