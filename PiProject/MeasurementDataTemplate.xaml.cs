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
using LiveCharts.Uwp;
using LiveCharts;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace PiProject
{
    public sealed partial class MeasurementDataTemplate : UserControl
    {
        public SeriesCollection SensorSeriesCollection { get; set; }
        public string TrainFlag { get; set; }

        public Measurement Measurement { get { return this.DataContext as Measurement; } }
        public MeasurementDataTemplate()
        {
            this.InitializeComponent();

            sensorChart.AxisX = new AxesCollection()
            {
                new Axis
                {
                    MinValue = 0,
                    MaxValue = 18,
                }
            };
            sensorChart.AxisY = new AxesCollection()
            {
                new Axis
                {
                    MinValue = 0
                }
            };

            DataContextChanged += MeasurementDataTemplate_DataContextChanged;
        }

        private void MeasurementDataTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if(this.DataContext != null)
            {
                SensorSeriesCollection = new SeriesCollection()
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
                TrainFlag = Measurement.IsTrain ? "Train" : "Test";

                Bindings.Update();
            }
        }
    }
}
