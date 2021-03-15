using System;
using System.Timers;

namespace FaceitStats.WPF.Classes
{
    class Error
    {
        private Timer timer;

        public string Message { get; set; }

        public Error(string message, int timerSeconds)
        {
            Message = message;
            timer = new Timer(timerSeconds * 1000);
            timer.Elapsed += (sender, e) =>
            {
                TimerElapsed?.Invoke(this, e);
                timer.Stop();
            };
            timer.Start();
        }

        public event EventHandler TimerElapsed;
    }
}
