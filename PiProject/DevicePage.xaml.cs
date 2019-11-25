using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using LiveCharts;
using LiveCharts.Uwp;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DevicePage : Page
    {
        int LedState { get; set; } = 0;
        public SeriesCollection SensorSeries { get; set; }

        public DevicePage()
        {
            this.InitializeComponent();

            LedState = 0;
            SensorSeries = new SeriesCollection();

            Loaded += DevicePage_Loaded;
        }

        private async void DevicePage_Loaded(object sender, RoutedEventArgs e)
        {
            await PiCup.ConnectDevice();
        }

        private async void SensorButton_Click(object sender, RoutedEventArgs e)
        {
            sensorButton.IsEnabled = false;

            await PiCup.ConnectDevice();

            var lst = await Settings.PiCup.ReadSensors(Settings.IntegrationTime);

            SensorSeries.Add(new LineSeries
            {
                Values = new ChartValues<float>(lst),
                Fill = new SolidColorBrush(Colors.Transparent)
            });

            Bindings.Update();

            sensorButton.IsEnabled = true;
        }

        private async void TempButton_Click(object sender, RoutedEventArgs e)
        {
            tempButton.IsEnabled = false;


            var lst = await Settings.PiCup.GetTemperature();

            tempDev0.Value = lst[0];
            tempDev1.Value = lst[1];
            tempDev2.Value = lst[2];

            tempButton.IsEnabled = true;
        }

        private async void LedSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            var obj = sender as ToggleSwitch;
            obj.IsEnabled = false;

            if (obj.Header.ToString().StartsWith("Vis"))
                LedState ^= 0x1;
            else if (obj.Header.ToString().StartsWith("IR"))
                LedState ^= 0x2;
            else if (obj.Header.ToString().StartsWith("Sensor"))
                LedState ^= 0x4;
            else
                LedState ^= 0x8;

            await PiCup.ConnectDevice();

            await Settings.PiCup.SetLedState(LedState);

            obj.IsEnabled = true;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            SensorSeries = new SeriesCollection();
            Bindings.Update();
        }
    }
}
