using System;
using System.Timers;

namespace FaceitStats.WPF.Classes
{
    class Notification
    {
        private Timer _timer;

        public string Message { get; set; }

        public Notification(string message, int timerSeconds)
        {
            Message = message;
            _timer = new Timer(timerSeconds * 1000);
            _timer.Elapsed += (sender, e) =>
            {
                TimerElapsed?.Invoke(this, e);
                _timer.Stop();
            };
            _timer.Start();
        }

        public event EventHandler TimerElapsed;
    }
}
