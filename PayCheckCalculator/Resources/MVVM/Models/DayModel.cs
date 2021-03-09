using System;
using System.Collections.Generic;

namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class DayModel
    {
        private DateTime _day;
        private DateTime _shiftStart;
        private DateTime _shiftEnd;
        private List<DateTime> _timeOptions;

        private TimeSpan _hours;
        
        public DateTime Day
        {
            get => _day;
            set => _day = value;
        }

        public DateTime ShiftStart
        {
            get => _shiftStart;
            set => _shiftStart = value;
        }

        public DateTime ShiftEnd
        {
            get => _shiftEnd;
            set => _shiftEnd = value;
        }

        public TimeSpan Hours
        {
            get => _hours;
            set => _hours = _shiftEnd - _shiftStart;
        }

        public List<DateTime> TimeOptions
        {
            get => _timeOptions;
            set => _timeOptions = value;
        }

        public DayModel(DateTime day, List<DateTime> timeOptions)
        {
            _day = day;
            _timeOptions = timeOptions;
        }
    }
}