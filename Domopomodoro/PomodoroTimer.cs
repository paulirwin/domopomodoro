using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Domopomodoro
{
    public sealed class PomodoroTimer
    {
        public enum PomodoroMode
        {
            Activity,
            ShortBreak,
            LongBreak
        }

        public class TimeChangedEventArgs : EventArgs
        {
            private readonly TimeSpan _newTime;
            private readonly double _percentComplete;

            public TimeChangedEventArgs(TimeSpan newTime, double percentComplete)
            {
                _newTime = newTime;
                _percentComplete = percentComplete;
            }

            public TimeSpan NewTime { get { return _newTime; } }

            public double PercentComplete { get { return _percentComplete; } }

            public string NewTimeFormatted
            {
                get { return _newTime.Minutes.ToString("0") + ":" + _newTime.Seconds.ToString("00"); }
            }
        }

        public class ModeChangedEventArgs : EventArgs
        {
            private readonly PomodoroMode _newMode;

            public ModeChangedEventArgs(PomodoroMode newMode)
            {
                _newMode = newMode;
            }

            public PomodoroMode NewMode { get { return _newMode; } }
        }

        private Stopwatch _stopwatch;
        private Timer _timer;
        private long _lastValue = 0;
        private int _completedPomodori = 0;
        private PomodoroMode _mode = PomodoroMode.Activity;

        public event EventHandler<TimeChangedEventArgs> TimeChanged;
        public event EventHandler<ModeChangedEventArgs> ModeChanged;
        public event EventHandler PomodoroCompleted;

        public PomodoroTimer()
        {
        }

        public void StartTimer()
        {
            if (_stopwatch == null)
                _stopwatch = Stopwatch.StartNew();
            else
                _stopwatch.Start();

            _timer = new Timer(TimerTick, null, 0, 250);
        }

        public void PauseTimer()
        {
            _stopwatch.Stop();

            StopTimerTicking();
        }

        public void ResetTimer()
        {
            _stopwatch.Reset();

            StopTimerTicking();

            ChangeMode(PomodoroMode.Activity);
            TimerTick(null);
        }

        public void ToggleTimer()
        {
            if (_stopwatch != null && _stopwatch.IsRunning)
                PauseTimer();
            else
                StartTimer();
        }

        private void StopTimerTicking()
        {
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
        }

        private void TimerTick(object state)
        {
            // 25 min = 1500 sec
            // 15 min = 900 sec
            // 5 min = 300 sec
            long newValue = (long)Math.Floor(_stopwatch.Elapsed.TotalSeconds);

            if (newValue == _lastValue)
                return;

            int mins = GetMinutesInCurrentMode();

            long finished = mins * 60;

            if (newValue >= finished)
            {
                _lastValue = finished;
                FinishCurrentSegment();

                var e2 = TimeChanged;

                if (e2 != null)
                    e2(this, new TimeChangedEventArgs(new TimeSpan(), 100.0));

                return;
            }

            _lastValue = newValue;
            TimeSpan timeRemaining = TimeSpan.FromMinutes(mins).Subtract(TimeSpan.FromSeconds(newValue));

            double percentageComplete = (100.0 * ((double)newValue / (mins * 60)));

            var e = TimeChanged;

            if (e != null)
                e(this, new TimeChangedEventArgs(timeRemaining, percentageComplete));
        }

        private void FinishCurrentSegment()
        {
            switch (_mode)
            {
                case PomodoroMode.Activity:
                    _completedPomodori++;

                    if (_completedPomodori % 4 == 0)
                    {
                        ChangeMode(PomodoroMode.LongBreak);
                    }
                    else
                    {
                        ChangeMode(PomodoroMode.ShortBreak);
                    }

                    var e = PomodoroCompleted;

                    if (e != null)
                        e(this, EventArgs.Empty);
                                        
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    break;
                case PomodoroMode.ShortBreak:
                case PomodoroMode.LongBreak:
                    ChangeMode(PomodoroMode.Activity);

                    _stopwatch.Reset();
                    break;
            }
        }

        private int GetMinutesInCurrentMode()
        {
            switch (_mode)
            {
                case PomodoroMode.Activity:
                    return 25;
                case PomodoroMode.ShortBreak:
                    return 5;
                case PomodoroMode.LongBreak:
                    return 15;
                default:
                    return 25;
            }
        }

        private void ChangeMode(PomodoroMode newMode)
        {
            _mode = newMode;

            var e = ModeChanged;

            if (e != null)
                e(this, new ModeChangedEventArgs(newMode));
        }

        public PomodoroMode Mode
        {
            get { return _mode; }
        }

        public int CompletedPomodori
        {
            get { return _completedPomodori; }
        }
    }
}
