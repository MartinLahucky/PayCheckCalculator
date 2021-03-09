using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class DayModel : INotifyPropertyChanged
    {
        private DateTime _day;
        private DateTime _shiftStart;
        private DateTime _shiftEnd;
        private List<DateTime> _timeOptions;
        public event PropertyChangedEventHandler PropertyChanged;
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
                NotifyPropertyChanged();
            }
        }

        public DateTime ShiftEnd
        {
            get => _shiftEnd;
            set
            {
                _shiftEnd = value;
                Hours = _shiftEnd - _shiftStart;
                NotifyPropertyChanged();
            }
        }

        public TimeSpan Hours
        {
            get => _shiftEnd - _shiftStart;
            set
            {
                value = _shiftEnd - _shiftStart;
                NotifyPropertyChanged();
            }
        }
        
        public List<DateTime> TimeOptions
        {
            get => _timeOptions;
            set => _timeOptions = value;
        }

        public DayModel(DateTime day)
        {
            _day = day;
        }
        
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}