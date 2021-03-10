using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppLocalization = PayCheckCalculator.Resources.Localization.Resources;

namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class DayModel : INotifyPropertyChanged
    {
        private DateTime _day;
        private DateTime _shiftStart;
        private DateTime _shiftEnd;
        private bool _shiftType;
        private List<DateTime> _timeOptions;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public DateTime Day
        {
            get => _day;
            set => _day = value;
        }

        public DateTime ShiftStart
        {
            get => _shiftStart;
            set
            {
                _shiftStart = value;
                Hours = new TimeSpan();
                NotifyPropertyChanged();
            }
        }

        public DateTime ShiftEnd
        {
            get => _shiftEnd;
            set
            {
                _shiftEnd = value;
                Hours = new TimeSpan();
                NotifyPropertyChanged();
            }
        }

        public bool ShiftType
        {
            get => _shiftType;
            set
            {
                _shiftType = value;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan Hours
        {
            get => _shiftEnd - _shiftStart;
            set => NotifyPropertyChanged();
        }

        public List<DateTime> TimeOptions
        {
            get => _timeOptions;
            set => _timeOptions = value;
        }

        public DayModel(DateTime day)
        {
            _day = day;
            _shiftType = true;
        }
    }
}