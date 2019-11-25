using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Sockets;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PiProject
{
    public sealed partial class DeviceConnectionDialog : ContentDialog
    {

        public bool IsDeviceConnected { get; set; } = false;
        private bool InProgress { get; set; } = false;

        private ObservableCollection<RfcommDeviceService> RfcommServiceList { get; set; }

        public DeviceConnectionDialog()
        {
            this.InitializeComponent();
            RfcommServiceList = new ObservableCollection<RfcommDeviceService>();

            Loaded += BluetoothConnectionDialog_Loaded;
        }

        private async void BluetoothConnectionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var loc_deviceInformationList = await DeviceInformation.FindAllAsync(
                    RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));

            foreach (var item in loc_deviceInformationList)
            {
                var service = await RfcommDeviceService.FromIdAsync(item.Id);
                RfcommServiceList.Add(service);

                Debug.WriteLine($"Service name: '{service.Device.Name}'");
            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (devicesList.SelectedItem != null)
            {
                if (InProgress)
                    return;
                InProgress = true;

                progressRing.IsActive = true;

                var service = (RfcommDeviceService)devicesList.SelectedItem;
                Debug.WriteLine($"HostName: {service.ConnectionHostName}\nServiceName:{service.ConnectionServiceName}\n");
                Debug.WriteLine($"Name:{service.Device.Name}\nHostName:{service.Device.HostName}");
                Debug.WriteLine($"ConnectionStatus:{service.Device.ConnectionStatus}");


                var picup = new PiCup(service);

                for (int i = 0; i < 3; ++i)
                {
                    progressTextBox.Text = $"Connecting... [{i + 1}/3]";
                    try
                    {
                        await picup.Connect();

                        Settings.PiCup = picup;
                        IsDeviceConnected = true;

                        Debug.WriteLine("EVERYTHING FINE!!!");
                        break;
                    }
                    catch (Exception ee)
                    {
                        Debug.WriteLine($"Error!\n{ee.Message}");
                    }
                }
                if (IsDeviceConnected)
                    this.Hide();
                else
                {
                    Settings.PiCup = null;

                    progressTextBox.Text = "Connection failed";
                    progressRing.IsActive = false;
                    devicesList.SelectedItem = null;

                    InProgress = false;
                }
            }
        }
    }
}
