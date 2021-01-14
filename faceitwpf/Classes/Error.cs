using System;
using System.Timers;

namespace faceitwpf.Classes
{
    class Error
    {
        private Timer timer;

        public string Message { get; set; }

        public Error(string message, int timerSeconds)
        {
            Message = message;
            timer = new Timer(timerSeconds * 1000);
            timer.Start();
            timer.Elapsed += (sender, e) =>
            {
                TimerElapsed?.Invoke(sender, e);
            };
            timer.Start();
        }

        public event EventHandler TimerElapsed;
    }
}
