using System;
using System.Collections.Generic;
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

        public async Task SaveExcelFileNoCopy(List<DayModel> data, string fileName, double shiftDayHours,
            double shiftNightHours, double shiftTotalHours, float dayWage, float nightWage, float totalWage)
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
            var cellRange = ws.Cells["A2"]
                .LoadFromCollection(FormatData(data, shiftDayHours, shiftNightHours, shiftTotalHours, dayWage, nightWage, totalWage));
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

        private List<ExcelModel> FormatData(List<DayModel> data, double shiftDayHours, double shiftNightHours,
            double shiftTotalHours, float dayWage, float nightWage, float totalWage)
        {
            var format = data.Select(day => new ExcelModel(day.Day.ToString("dd. MM. yyyy"),
                    day.ShiftStart.ToString("HH:mm"), day.ShiftEnd.ToString("HH:mm"), day.Hours.TotalHours,
                    day.ShiftType))
                .ToList();

            format.Add(new ExcelModel($"{AppLocalization.Total} {AppLocalization.Hours}: {shiftDayHours}h",
                string.Empty, $"{AppLocalization.ShiftNight}: {shiftNightHours}h", null,
                $"{AppLocalization.ShiftDay}: {shiftTotalHours}h"));
            format.Add(new ExcelModel($"{AppLocalization.Total} {AppLocalization.Hours}: {dayWage} Kč",
                string.Empty, $"{AppLocalization.ShiftNight}: {nightWage} Kč", null,
                $"{AppLocalization.ShiftDay}: {totalWage} Kč"));
            return format;
        }

        private FileInfo CreatePath(string fileName)
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            return new FileInfo($@"{desktopPath}\{fileName}.xlsx");
        }

        public async Task<List<DayModel>> LoadDataFromExcel(FileInfo file)
        {
            // This is a free open source project 
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var loadedData = new List<DayModel>();
            using var package = new ExcelPackage(file);
            await package.LoadAsync(file);
            var ws = package.Workbook.Worksheets[0];
            int row = 2, col = 1;

            while (string.IsNullOrWhiteSpace(ws.Cells[row, col].Value?.ToString()) == false)
            {
                var day = new DayModel(
                    DateTime.Parse(ws.Cells[row, col].Value.ToString()!),
                    DateTime.Parse(ws.Cells[row, col + 1].Value.ToString()!),
                    DateTime.Parse(ws.Cells[row, col + 2].Value.ToString()!),
                    ws.Cells[row, col + 4].Value.ToString());
                SetDay(day);
                loadedData.Add(day);
                row++;
            }

            return loadedData;
        }

        private void DeleteIfExists(FileInfo file)
        {
            if (file.Exists) file.Delete();
        }

        public List<DayModel> GetData(int year, int month)
        {
            return GetDates(year, month).Select(day => SetDay(new DayModel(day))).ToList();
        }

        private List<DateTime> GetDates(int year, int month)
        {
            return Enumerable.Range(1, DateTime.DaysInMonth(year, month)) // Days: 1, 2 ... 31 etc.
                .Select(day => new DateTime(year, month, day)) // Map each day to a date
                .ToList(); // Load dates into a list
        }

        private DayModel SetDay(DayModel day)
        {
            var list = new List<DateTime>();
            var time = day.Day;
            for (var i = 0; i < 48; i++)
            {
                switch (i)
                {
                    case 7:
                        day.ShiftStart = time;
                        break;
                    case 17:
                        day.ShiftEnd = time;
                        break;
                }

                for (var j = 0; j < 4; j++)
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