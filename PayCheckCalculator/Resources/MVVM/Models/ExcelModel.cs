using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class ExcelModel : INotifyPropertyChanged
    {
        private string _day;
        private string _shiftStart;
        private string _shiftEnd;
        private double? _hours;
        private string _shiftType;
        
        public ExcelModel(string day, string shiftStart, string shiftEnd, double? hours, string shiftType)
        {
            _day = day;
            _shiftStart = shiftStart;
            _shiftEnd = shiftEnd;
            _hours = hours;
            _shiftType = shiftType;
        }

        public string Day
        {
            get => _day;
            set
            {
                _day = value;
                NotifyPropertyChanged();
            }
        }

        public string ShiftStart
        {
            get => _shiftStart;
            set
            {
                _shiftStart = value;
                NotifyPropertyChanged();
            }
        }

        public string ShiftEnd
        {
            get => _shiftEnd;
            set
            {
                _shiftEnd = value;
                NotifyPropertyChanged();
            }
        }

        public double? Hours
        {
            get => _hours;
            set
            {
                _hours = value;
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

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}