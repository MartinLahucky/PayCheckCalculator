using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using PayCheckCalculator.Resources.MVVM.Models;
using AppLocalization = PayCheckCalculator.Resources.Localization.Resources;

namespace PayCheckCalculator.Resources.Functions
{
    public class ExcelFunctions
    {
        public async Task SaveExcelFile(FileInfo file, List<DayModel> data, string fileName)
        {
            // This is a free open source project 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


            // DeleteIfExists(file);
            file = RenameIfExists(file, fileName);

            using var package = new ExcelPackage(file);
            var ws = package.Workbook.Worksheets.Add("DataSheet");
            var cellRange = ws.Cells["A1"].LoadFromCollection(data, true);
            cellRange.AutoFitColumns();
            await package.SaveAsync();
        }

        public async Task SaveExcelFileNoCopy(List<DayModel> data, string fileName)
        {
            // This is a free open source project 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var file = CreatePath(fileName);
            DeleteIfExists(file);


            using var package = new ExcelPackage(file);
            var ws = package.Workbook.Worksheets.Add("DataSheet");
            ws.Cells["A1"].Value = AppLocalization.Day;
            ws.Cells["B1"].Value = AppLocalization.ShiftStart;
            ws.Cells["C1"].Value = AppLocalization.ShiftEnd;
            ws.Cells["D1"].Value = AppLocalization.Hours;
            ws.Cells["E1"].Value = AppLocalization.ShiftType;
            var cellRange = ws.Cells["A2"].LoadFromCollection(FormatData(data));
            cellRange.AutoFitColumns();
            ws.Cells["B1:C1"].AutoFitColumns();
            await package.SaveAsync();
        }

        private FileInfo RenameIfExists(FileInfo file, string fileName)
        {
            byte id = 0;
            while (file.Exists)
            {
                file = CreatePath($"{fileName}{id}");
                id++;
            }

            return file;
        }

        private List<ExcelModel> FormatData(List<DayModel> data)
        {
            var format = new List<ExcelModel>();
            string shiftType = AppLocalization.ShiftDay;
            TimeSpan totalHours = new();

            foreach (var day in data)
            {
                if (!day.ShiftType)
                {
                    shiftType = AppLocalization.ShiftNight;
                }

                totalHours += day.Hours;

                format.Add(new ExcelModel(day.Day.ToString("dd. MM. yyyy"),
                    day.ShiftStart.ToString("HH:mm"),
                    day.ShiftEnd.ToString("HH:mm"),
                    day.Hours.TotalHours,
                    shiftType));
            }

            format.Add(new ExcelModel(string.Empty, string.Empty, string.Empty, totalHours.TotalHours, string.Empty));
            return format;
        }

        private FileInfo CreatePath(string fileName)
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return new($@"{desktopPath}\{fileName}.xlsx");
        }


        private void DeleteIfExists(FileInfo file)
        {
            if (file.Exists) file.Delete();
        }

        public List<DayModel> GetData(int year, int month)
        {
            List<DayModel> list = new List<DayModel>();
            foreach (var day in GetDates(year, month))
            {
                list.Add(SetDay(new DayModel(day)));
            }

            return list;
        }

        private List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month)) // Days: 1, 2 ... 31 etc.
                .Select(day => new DateTime(year, month, day)) // Map each day to a date
                .ToList(); // Load dates into a list
        }

        // TODO REWORK 
        private DayModel SetDay(DayModel day)
        {
            List<DateTime> list = new List<DateTime>();
            var time = day.Day;
            for (int i = 0; i < 48; i++)
            {
                if (i == 0)
                {
                    day.ShiftStart = time;
                }
                else if (i == 13)
                {
                    day.ShiftEnd = time;
                }

                for (int j = 0; j < 4; j++)
                {
                    list.Add(time);
                    time = time.AddMinutes(15);
                }
            }

            day.TimeOptions = list;
            return day;
        }
    }
}