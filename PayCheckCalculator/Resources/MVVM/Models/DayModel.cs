using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using AppLocalization = PayCheckCalculator.Resources.Localization.Resources;

namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class DayModel : INotifyPropertyChanged
    {
        private DateTime _shiftEnd;
        private DateTime _shiftStart;
        private string _shiftType;

        public DayModel(DateTime day)
        {
            Day = day;
            _shiftType = AppLocalization.ShiftDay;
        }

        public DayModel(DateTime day, DateTime shiftStart, DateTime shiftEnd, string shiftType)
        {
            _shiftEnd = shiftEnd;
            _shiftStart = shiftStart;
            _shiftType = shiftType;
            Day = day;
        }

        public DateTime Day { get; set; }

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

        public string ShiftType
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

        public List<DateTime> TimeOptions { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}