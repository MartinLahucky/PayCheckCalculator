using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using PayCheckCalculator.Resources.MVVM.Models;

namespace PayCheckCalculator.Resources.Functions
{
    public class ExcelFunctions
    {
        // public async Task SaveExcelFile(FileInfo file, List<PayCheckModel> data, string fileName)
        // {
        //     // This is a free open source project 
        //     ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //     
        //     
        //     // DeleteIfExists(file);
        //     file = RenameIfExists(file, fileName);
        //
        //     using var package = new ExcelPackage(file);
        //     var ws = package.Workbook.Worksheets.Add("DataSheet");
        //     var cellRange = ws.Cells["A1"].LoadFromCollection(data, true);
        //     cellRange.AutoFitColumns();
        //     await package.SaveAsync();
        // }

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
        
        private DayModel SetDay(DayModel day)
        {
            List<DateTime> list = new List<DateTime>();
            var time = day.Day;
            for (int i = 0; i < 24; i++)
            {
                if (i == 7)
                {
                    day.ShiftStart = time;
                }
                else if (i == 17)
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