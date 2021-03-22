namespace PayCheckCalculator.Resources.MVVM.Models
{
    public class ExcelModel
    {
        public ExcelModel(string day, string shiftStart, string shiftEnd, double hours, string shiftType)
        {
            Day = day;
            ShiftStart = shiftStart;
            ShiftEnd = shiftEnd;
            Hours = hours;
            ShiftType = shiftType;
        }

        public string Day { get; set; }
        public string ShiftStart { get; set; }
        public string ShiftEnd { get; set; }
        public double Hours { get; set; }
        public string ShiftType { get; set; }
    }
}