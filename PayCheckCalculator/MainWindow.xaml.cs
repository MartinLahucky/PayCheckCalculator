using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using PayCheckCalculator.Resources.Functions;
using PayCheckCalculator.Resources.MVVM.Models;

namespace PayCheckCalculator
{
    public partial class MainWindow : Window
    {
        private readonly ExcelFunctions _excel = new ExcelFunctions();
        private List<DayModel> _data;

        public MainWindow()
        {
            InitializeComponent();

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
            var time = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
            _data[time.Day - 1].ShiftStart = time;
        }

        private void OptionsTo_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var time = DateTime.Parse(e.AddedItems[0]?.ToString() ?? string.Empty);
            _data[time.Day - 1].ShiftEnd = time;
        }
    }
}