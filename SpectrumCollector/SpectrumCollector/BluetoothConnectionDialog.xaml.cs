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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SpectrumCollector
{
    public sealed partial class BluetoothConnectionDialog : ContentDialog
    {
        public StreamSocket ConnectionSocket { get; set; }

        public ObservableCollection<DeviceInformation> deviceInformationList;
        public BluetoothConnectionDialog()
        {
            this.InitializeComponent();

            ConnectionSocket = null;
            deviceInformationList = new ObservableCollection<DeviceInformation>();

            Loaded += BluetoothConnectionDialog_Loaded;
        }

        private async void BluetoothConnectionDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var loc_deviceInformationList = await DeviceInformation.FindAllAsync(
                    RfcommDeviceService.GetDeviceSelector(RfcommServiceId.SerialPort));
            foreach (var item in loc_deviceInformationList)
            {
                deviceInformationList.Add(item);
            }
        }

        private async void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (devicesList.SelectedItem != null)
            {
                progressRing.IsActive = true;
                try
                {
                    var selected = (DeviceInformation)devicesList.SelectedItem;
                    var service = await RfcommDeviceService.FromIdAsync(selected.Id);

                    Debug.WriteLine($"HostName: {service.ConnectionHostName}\nServiceName:{service.ConnectionServiceName}\n");
                    Debug.WriteLine($"Name:{service.Device.Name}\nHostName:{service.Device.HostName}");
                    Debug.WriteLine($"ConnectionStatus:{service.Device.ConnectionStatus}");

                    progressTextBox.Text = "Connecting";

                    var socket = new StreamSocket();

                    await socket.ConnectAsync(service.ConnectionHostName,
                        service.ConnectionServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                    if (socket != null)
                    {
                        ConnectionSocket = socket;
                        this.Hide();
                    }
                    else
                    {
                        progressTextBox.Text = "Connection failed";
                    }
                }
                catch (Exception ee)
                {
                    await new Windows.UI.Popups.MessageDialog($"Exception while connecting: '{ee.Message}'").ShowAsync();
                    progressTextBox.Text = "Connection failed";
                }
                progressRing.IsActive = false;
                devicesList.SelectedItem = null;
            }
        }
    }
}
