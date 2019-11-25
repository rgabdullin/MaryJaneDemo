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
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;


// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiProject
{
    public sealed partial class NewMeasurementDialog : ContentDialog
    {
        public static string PrevDataset { get; set; } = null;
        public bool IsMeasurementsSuccessful { get; private set; } = false;
        public bool SaveMeasurementFlag { get; set; } = false;

        public Measurement Measurement { get; private set; } = null;
        public NewMeasurementDialog()
        {
            this.InitializeComponent();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private async void DatasetComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                if(db.Datasets.ToList().Count == 0)
                {
                    var default_dataset = new Dataset
                    {
                        Name = "default",
                        Description = "Default dataset",
                    };
                    db.Datasets.Add(default_dataset);
                    db.SaveChanges();
                }
            }
            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                var datasetList = db.Datasets.Include("Labels").OrderBy(a => a.Name).ToList();
                datasetComboBox.ItemsSource = datasetList;
            }

            datasetComboBox.SelectedIndex = 0;
        }

        private void DatasetComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(datasetComboBox.SelectedItem != null)
            {
                observeButton.IsEnabled = true;
                if(testToggleSwitch.IsOn)
                    trainParams.Visibility = Visibility.Visible;
                labelSuggestBox.ItemsSource = (datasetComboBox.SelectedItem as Dataset).Labels;
                // labelSuggestBox.IsSuggestionListOpen = true;
            }
            else
            {
                observeButton.IsEnabled = false;
                trainParams.Visibility = Visibility.Collapsed;
            }
        }
        private async void ObserveButton_Click(object sender, RoutedEventArgs e)
        {
            observeButton.IsEnabled = false;
            observeProgress.IsActive = true;

            {
                var dataset = datasetComboBox.SelectedItem as Dataset;
                var labelText = labelSuggestBox.Text == "" ? "auto" : labelSuggestBox.Text;

                Label label = null;
                if (testToggleSwitch.IsOn)
                {
                    label = dataset.Labels.Where(a => a.Name == labelText).FirstOrDefault();

                    if (label == null)
                    {
                        using (var db = new DatabaseContext(Settings.SqlOptions))
                        {
                            var db_dataset = db.Datasets.Where(a => a.Id == dataset.Id).First();
                            label = new Label
                            {
                                Name = labelText,
                                Dataset = dataset,
                                Description = descriptionTextBox.Text,
                            };

                            db.Labels.Add(label);
                            db.SaveChanges();
                        }
                    }
                }

                var picup = Settings.PiCup;

                Measurement = await picup.GetMeasurement();
                Measurement.Label = label;
                Measurement.IsTrain = testToggleSwitch.IsOn;
                using (var db = new DatabaseContext(Settings.SqlOptions))
                {
                    Measurement.Dataset = db.Datasets.Where(a => a.Id == dataset.Id).First();
                    if (Measurement.IsTrain)
                    {
                        Measurement.Label = db.Labels.Where(a => a.Id == label.Id).First();
                    }
                    else
                    {
                        // var labelName = await MachineLearningWebService.ScoreMeasurement(Measurement);
                        // Measurement.Label = db.Labels.Where(a => (a.Name == labelName) && (a.DatasetId == dataset.Id)).First();
                        Measurement.Label = null;
                    }

                    if (Measurement.IsTrain || (saveTestMeasurementChecker.IsChecked == true))
                    {
                        db.Measurements.Add(Measurement);
                        db.SaveChanges();
                    }
                }

                IsMeasurementsSuccessful = true;
                SaveMeasurementFlag = Measurement.IsTrain || (saveTestMeasurementChecker.IsChecked == true);
                this.Hide();

            }

            observeButton.IsEnabled = true;
            observeProgress.IsActive = false;
        }

        private void TestToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {

            if (testToggleSwitch.IsOn)
            {
                if (datasetComboBox.SelectedItem != null)
                    trainParams.Visibility = Visibility.Visible;
                else
                    trainParams.Visibility = Visibility.Collapsed;
                testParams.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (datasetComboBox.SelectedItem != null)
                    trainParams.Visibility = Visibility.Collapsed;
                else
                    trainParams.Visibility = Visibility.Visible;
                testParams.Visibility = Visibility.Visible;
            }
        }

        private void LabelSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                var label = db.Labels.Where(a => a.Name == labelSuggestBox.Text).FirstOrDefault();
                if (label != null)
                {
                    descriptionTextBox.Text = label.Description ?? "Null description";
                    descriptionTextBox.IsEnabled = false;
                }
                else
                {
                    descriptionTextBox.Text = "";
                    descriptionTextBox.IsEnabled = true;
                }
            }
        }
    }
}
