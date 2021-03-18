using FaceitStats.WPF.Classes;
using FaceitStats.WPF.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace FaceitStats.WPF.Services
{
    class NotifyService: INotifyService
    {
        public void DisplayError(Exception exception)
        {
            SetError(exception);
        }

        private void SetError(Exception exception)
        {
            //Error = new Error(exception.Message, 3);
            //Error.TimerElapsed += (sender, e) =>
            //{
            //    if (sender.Equals(Error))
            //        Error = null;
            //};
        }
    }
}
