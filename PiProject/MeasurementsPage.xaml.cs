using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiProject
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MeasurementsPage : Page
    {
        ObservableCollection<Measurement> MeasurementList { get; set; }

        public MeasurementsPage()
        {
            this.InitializeComponent();

            MeasurementList = new ObservableCollection<Measurement>();
            measGrid.Loaded += MeasurementListPage_Loaded;
        }
        private async void MeasurementListPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateMeasurementList();
        }

        private async void NewMeasurementButton_Click(object sender, RoutedEventArgs e)
        {
            await PiCup.ConnectDevice();

            var measDialog = new NewMeasurementDialog();
            await measDialog.ShowAsync();

            if (measDialog.IsMeasurementsSuccessful)
            {
                var mes = measDialog.Measurement;

                if(measDialog.SaveMeasurementFlag)
                    MeasurementList.Add(mes);

                if (!mes.IsTrain)
                    await new MeasurementDetails(mes).ShowAsync();
            }
        }

        private void RefreshMeasurementListButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateMeasurementList();
        }

        private void UpdateMeasurementList()
        {
            try
            {
                using (var db = new DatabaseContext(Settings.SqlOptions))
                {
                    var lst = db.Measurements
                        .Include("Data")
                        .Include("Dataset")
                        .Include("Label")
                        .OrderBy(a => a.CreationStamp)
                        .ToList();
                    MeasurementList = new ObservableCollection<Measurement>(lst);

                    Bindings.Update();
                }
            }
            catch (Exception ee)
            {
                //Task.Run(async () => await new Windows.UI.Popups.MessageDialog($"Exception in UpdateMeasurementList:\n{ee.Message}").ShowAsync()).Wait();
            }
        }

        private void DeleteMeasurementButton_Click(object sender, RoutedEventArgs e)
        {
            var mesList = from obj in measGrid.SelectedItems select obj as Measurement;

            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                db.Measurements.RemoveRange(mesList);
                db.SaveChanges();

                foreach (var label in db.Labels.Include("Measurements"))
                    if (label.Measurements.Count == 0)
                        db.Labels.Remove(label);
                db.SaveChanges();

                foreach (var dataset in db.Datasets.Include("Labels"))
                    if (dataset.Labels.Count == 0)
                        db.Datasets.Remove(dataset);
                db.SaveChanges();
            }
            UpdateMeasurementList();

            measGrid.SelectedItem = null;
        }

        private void SelectAllButton_Click(object sender, RoutedEventArgs e)
        {
            measGrid.SelectAll();
        }

        private void MeasGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (measGrid.SelectedItems.Count == 0)
            {
                deleteMeasurementButton.IsEnabled = false;
            }
            else
            {
                deleteMeasurementButton.IsEnabled = true;
            }
        }

        private void NewDatasetNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = newDatasetNameTextBox.Text;
            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                if ((text == "") || (db.Datasets.Where(a => a.Name == text).ToList().Count() > 0))
                    newDatasetCreateButton.IsEnabled = false;
                else
                    newDatasetCreateButton.IsEnabled = true;
            }
        }

        private void NewDatasetCreateButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DatabaseContext(Settings.SqlOptions))
            {
                var dataset = new Dataset
                {
                    Name = newDatasetNameTextBox.Text,
                    Description = "No description yet"
                };
                db.Datasets.Add(dataset);
                db.SaveChanges();
            }

            newDatasetCreateButton.IsEnabled = false;
            newDatasetNameTextBox.Text = "";
            newDatasetFlyout.Hide();
        }
        private async void MeasGrid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var mes = (e.OriginalSource as FrameworkElement)?.DataContext as Measurement;

            if (mes == null)
                return;

            var dialog = new MeasurementDetails(mes);
            await dialog.ShowAsync();
        }
    }
}
