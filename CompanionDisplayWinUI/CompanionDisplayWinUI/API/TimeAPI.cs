using CompanionDisplayWinUI.ClassImplementations;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Threading;

namespace CompanionDisplayWinUI.API
{
    public partial class TimeAPI : INotifyPropertyChanged
    {
        private string _currentTime;
        public string CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTime));
                }
            }
        }
        private string _currentTimeSecs;
        public string CurrentTimeSecs
        {
            get => _currentTimeSecs;
            set
            {
                if (_currentTimeSecs != value)
                {
                    _currentTimeSecs = value;
                    OnPropertyChanged(nameof(CurrentTimeSecs));
                }
            }
        }

        private string _currentDate;
        public string CurrentDate
        {
            get => _currentDate;
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    OnPropertyChanged(nameof(CurrentDate));
                }
            }
        }
        private string _currentDateAbbr;
        public string CurrentDateAbbr
        {
            get => _currentDateAbbr;
            set
            {
                if (_currentDateAbbr != value)
                {
                    _currentDateAbbr = value;
                    OnPropertyChanged(nameof(CurrentDateAbbr));
                }
            }
        }

        public TimeAPI()
        {
            DispatcherTimer timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += (s, e) =>
            {
                if (Globals.use12HourClock)
                {
                    CurrentTime = DateTime.Now.ToString("h:mm tt", CultureInfo.CurrentUICulture).ToLower();
                    CurrentTimeSecs = DateTime.Now.ToString("h:mm:ss tt", CultureInfo.CurrentUICulture).ToLower();
                }
                else
                {
                    CurrentTime = DateTime.Now.ToString("HH:mm", CultureInfo.CurrentUICulture);
                    CurrentTimeSecs = DateTime.Now.ToString("HH:mm:ss", CultureInfo.CurrentUICulture);
                }
                CurrentDate = DateTime.Now.Date.ToString("dddd, dd MMMM yyyy", CultureInfo.CurrentUICulture);
                CurrentDateAbbr = DateTime.Now.Date.ToString("dd/MM/yyyy", CultureInfo.CurrentUICulture);
            };
            timer.Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
