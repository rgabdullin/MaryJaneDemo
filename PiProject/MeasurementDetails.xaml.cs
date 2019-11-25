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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiProject
{
    public sealed partial class MeasurementDetails : ContentDialog
    {
        public Measurement Measurement { get; set; }

        public SeriesCollection SensorSeries { get; set; }

        public MeasurementDetails(Measurement mes)
        {
            this.InitializeComponent();

            this.Measurement = mes;

            if (mes != null)
            {

                SensorSeries = new SeriesCollection()
                {
                    new LineSeries
                        {
                            Values = new ChartValues<float>(
                                from obj in Measurement.Data
                                where obj.LedId == 0
                                orderby obj.LedId, obj.Freq
                                select obj.Value),
                            Fill = new SolidColorBrush(Windows.UI.Colors.Transparent),
                            Title = "Visible LED",
                        },
                        new LineSeries
                        {
                            Values = new ChartValues<float>(
                                from obj in Measurement.Data
                                where obj.LedId == 1
                                orderby obj.LedId, obj.Freq
                                select obj.Value),
                            Fill = new SolidColorBrush(Windows.UI.Colors.Transparent),
                            Title = "IR LED",
                        },
                };

                Bindings.Update();
            }
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            this.Hide();
        }
    }
}
