using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using PayCheckCalculator.Resources.Functions;
using PayCheckCalculator.Resources.MVVM.Models;
using AppLocalization = PayCheckCalculator.Resources.Localization.Resources;

namespace PayCheckCalculator
{
    public partial class MainWindow : Window
    {
        private readonly ExcelFunctions _excel = new();
        private List<DayModel> _data;

        public MainWindow()
        {
            InitializeComponent();

            var currentYear = int.Parse(DateTime.Now.Year.ToString());

            for (var i = 50; i > -50; i--) YearOptions.Items.Add((currentYear + i).ToString());

            YearOptions.SelectedIndex = YearOptions.Items.IndexOf(DateTime.Now.Year.ToString());
            MonthOptions.SelectedIndex = DateTime.Now.Month - 1;
            LoadData();
        }

        private void LoadData(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            try
            {
                LoadData();
            }
            catch
            {
                // ignored
            }
        }

        private void LoadData()
        {
            if (YearOptions.SelectedItem.ToString() == null || MonthOptions.SelectedItem.ToString() == null) return;
            var year = int.Parse(YearOptions.SelectedItem.ToString() ?? "0");
            var month = MonthOptions.SelectedIndex + 1;
            _data = _excel.GetData(year, month);
            DayOfTheMonthList.ItemsSource = _data;
        }

        private void OptionsBegin_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = (ComboBox) sender;
            var id = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
            _data[id.Day - 1].ShiftStart = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
        }

        private void OptionsTo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = (ComboBox) sender;
            var id = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
            _data[id.Day - 1].ShiftEnd = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
        }

        private void ShiftType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = (ComboBox) sender;
            var content = (ComboBoxItem) e.AddedItems[0];
            if (box != null && content != null)
            {
                var time = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
                if (content.Content.ToString() == AppLocalization.ShiftDay)
                {
                    _data[time.Day - 1].ShiftType = AppLocalization.ShiftDay;
                    _data[time.Day - 1].ShiftStart = _data[time.Day - 1].TimeOptions[28];
                    _data[time.Day - 1].ShiftEnd = _data[time.Day - 1].TimeOptions[68];
                }
                else if (content.Content.ToString() == AppLocalization.ShiftNight)
                {
                    _data[time.Day - 1].ShiftType = AppLocalization.ShiftNight;
                    _data[time.Day - 1].ShiftStart = _data[time.Day - 1].TimeOptions[80];
                    _data[time.Day - 1].ShiftEnd = _data[time.Day - 1].TimeOptions[120];
                }
                else if (content.Content.ToString() == AppLocalization.FreeDay)
                {
                    _data[time.Day - 1].ShiftType = AppLocalization.FreeDay;
                    _data[time.Day - 1].ShiftStart = _data[time.Day - 1].TimeOptions[0];
                    _data[time.Day - 1].ShiftEnd = _data[time.Day - 1].TimeOptions[0];
                }
            }
        }

        private async void CalculateButton_OnClick(object sender, RoutedEventArgs e)
        {
            // ShiftDay.Content = null;
            // ShiftNight.Content = null;
            // ShiftTotal.Content = null;
            float dayWage, nightWage;
            TimeSpan shiftDay = new(), shiftNight = new();

            switch (!String.IsNullOrEmpty(ShiftDayWage.Text), !String.IsNullOrEmpty(ShiftNightWage.Text))
            {
                case (true, true):
                    dayWage = float.Parse(ShiftDayWage.Text);
                    nightWage = float.Parse(ShiftNightWage.Text);
                    break;

                case (true, false):
                    dayWage = float.Parse(ShiftDayWage.Text);
                    nightWage = float.Parse(ShiftNightWage.Tag.ToString() ?? string.Empty);
                    break;

                case (false, true):
                    dayWage = float.Parse(ShiftDayWage.Tag.ToString() ?? string.Empty);
                    nightWage = float.Parse(ShiftNightWage.Text);
                    break;

                case (false, false):
                    dayWage = float.Parse(ShiftDayWage.Tag.ToString() ?? string.Empty);
                    nightWage = float.Parse(ShiftNightWage.Tag.ToString() ?? string.Empty);
                    break;
            }

            foreach (var day in _data)
                if (day.ShiftType == AppLocalization.ShiftDay)
                    shiftDay += day.Hours;
                else if (day.ShiftType == AppLocalization.ShiftNight)
                    shiftNight += day.Hours;
            dayWage = (float) shiftDay.TotalHours * dayWage;
            nightWage = (float) shiftNight.TotalHours * nightWage;
            var totalWage = dayWage + nightWage;

            DayHoursLabel.Content = $"{shiftDay.TotalHours} {AppLocalization.Hours}";
            DayWageLabel.Content = $"{dayWage} Kč";
            NightHoursLabel.Content = $"{shiftNight.TotalHours} {AppLocalization.Hours}";
            NightWageLabel.Content = $"{nightWage} Kč";
            TotalHoursLabel.Content = $"{shiftDay.TotalHours + shiftNight.TotalHours} {AppLocalization.Hours}";
            TotalWageLabel.Content = $"{totalWage} Kč";
            await _excel.SaveExcelFileNoCopy(_data, $"{MonthOptions.Text}_{YearOptions.Text}", shiftDay.TotalHours,
                shiftNight.TotalHours, shiftDay.TotalHours + shiftNight.TotalHours, dayWage, nightWage, totalWage);
        }

        private async void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            var fileName = new FileInfo(GetFile());
            var data = await _excel.LoadDataFromExcel(fileName);
            YearOptions.SelectedIndex = YearOptions.Items.IndexOf(data[0].Day.Year.ToString());
            MonthOptions.SelectedIndex = data[0].Day.Month - 1;
            DayOfTheMonthList.ItemsSource = data;
        }

        private string GetFile()
        {
            var findFile = new OpenFileDialog
            {
                Title = AppLocalization.FindAFile,
                Filter = $"{AppLocalization.ExcelDocument} (*.xlsx) | *.xlsx",
                FileName = " "
            };

            return findFile.ShowDialog() == true ? findFile.FileName : GetFile();
        }
    }
}