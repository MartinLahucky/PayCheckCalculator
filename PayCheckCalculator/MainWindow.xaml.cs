using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using PayCheckCalculator.Resources.Functions;
using PayCheckCalculator.Resources.MVVM.Models;
using AppLocalization = PayCheckCalculator.Resources.Localization.Resources;

namespace PayCheckCalculator
{
    public partial class MainWindow : Window
    {
        private readonly ExcelFunctions _excel = new ExcelFunctions();
        private List<DayModel> _data;

        public MainWindow()
        {
            InitializeComponent();
            ShiftDayWageLabel.Content = $"{AppLocalization.HourlyWage} - {AppLocalization.ShiftDay}";
            ShiftNightWageLabel.Content = $"{AppLocalization.HourlyWage} - {AppLocalization.ShiftNight}";

            var currentYear = int.Parse(DateTime.Now.Year.ToString());

            for (int i = 50; i > -50; i--)
            {
                YearOptions.Items.Add((currentYear + i).ToString());
            }

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
            }
        }

        private void LoadData()
        {
            if (YearOptions.SelectedItem.ToString() != null && MonthOptions.SelectedItem.ToString() != null)
            {
                var year = int.Parse(YearOptions.Text);
                var month = MonthOptions.SelectedIndex + 1;
                _data = _excel.GetData(year, month);
                DayOfTheMonthList.ItemsSource = _data;
            }
        }

        private void OptionsBegin_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox) sender;
            var id = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
            _data[id.Day - 1].ShiftStart = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
        }

        private void OptionsTo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox) sender;
            var id = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
            _data[id.Day - 1].ShiftEnd = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
        }


        private void ShiftType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox box = (ComboBox) sender;
            if (box != null)
            {
                var time = DateTime.Parse(box.Tag.ToString() ?? string.Empty);
                if (box.Text == AppLocalization.ShiftDay)
                {
                    _data[time.Day - 1].ShiftType = false;
                }
                else if (box.Text == AppLocalization.ShiftNight)
                {
                    _data[time.Day - 1].ShiftType = true;
                }
            }
        }

        private async void CalculateButton_OnClick(object sender, RoutedEventArgs e)
        {
            ShiftDay.Content = null;
            ShiftNight.Content = null;
            ShiftTotal.Content = null;
            float dayWage, nightWage;
            TimeSpan shiftDay = new(), shiftNight = new();

            switch (Regex.IsMatch(ShiftDayWage.Text, "[^0-9]+"), Regex.IsMatch(ShiftNightWage.Text, "[^0-9]+"))
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
            {
                if (!day.ShiftType)
                {
                    shiftNight += day.Hours;
                }
                else
                {
                    shiftDay += day.Hours;
                }
            }
            dayWage = (float) shiftDay.TotalHours * dayWage;
            nightWage = (float) shiftNight.TotalHours * nightWage;
            float total = dayWage + nightWage; 
            ShiftDay.Content = $"{AppLocalization.ShiftDay}: {shiftDay.TotalHours} {AppLocalization.Hours} -> {dayWage} $";
            ShiftNight.Content =
                $"{AppLocalization.ShiftNight}: {shiftNight.TotalHours} {AppLocalization.Hours} -> {nightWage} $";
            ShiftTotal.Content =
                $"{AppLocalization.Total}: {shiftDay.TotalHours + shiftNight.TotalHours} {AppLocalization.Hours} -> {total} $";
            await _excel.SaveExcelFileNoCopy(_data, $"{MonthOptions.Text}_{YearOptions.Text}");
        }
    }
}