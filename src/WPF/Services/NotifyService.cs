using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using System;

namespace FaceitStats.WPF.Services
{
    class NotifyService : INotifyService
    {
        public Notification Error { get; private set; }

        public void DisplayError(Exception exception)
        {
            SetError(exception);
        }

        private void SetError(Exception exception)
        {
            Error = new Notification(exception.Message, 3);
            Error.TimerElapsed += (sender, e) =>
            {
                if (sender.Equals(Error))
                {
                    Error = null;
                    NotificationRemoved?.Invoke(this, new EventArgs());
                }
            };
            NotificationCreated?.Invoke(this, new NotificationCreatedEventArgs(Error));
        }

        public event EventHandler<NotificationCreatedEventArgs> NotificationCreated;
        public event EventHandler NotificationRemoved;
    }

    class NotificationCreatedEventArgs : EventArgs
    {
        public Notification Notification { get; set; }

        public NotificationCreatedEventArgs(Notification notification)
        {
            Notification = notification;
        }
    }
}
