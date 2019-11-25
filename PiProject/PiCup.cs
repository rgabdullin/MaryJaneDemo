using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace PiProject
{
    public class PiCup
    {
        float[] _sensorState = new float[18];

        bool _isBusy = false;

        private RfcommDeviceService Service { get; set; }
        private StreamSocket Socket { get; set; }
        private DataWriter Writer { get; set; }
        private DataReader Reader { get; set; }

        public PiCup(RfcommDeviceService service)
        {
            
            Service = service;
            Socket = null;
            Reader = null;
            Writer = null;

            _isBusy = false;

            Service.Device.ConnectionStatusChanged += Device_ConnectionStatusChanged;
        }

        private async void Device_ConnectionStatusChanged(BluetoothDevice sender, object args)
        {
            _isBusy = true;

            Debug.WriteLine($"WARNING! ConnectionStatus: {Service.Device.ConnectionStatus}");

            if (Service.Device.ConnectionStatus == BluetoothConnectionStatus.Disconnected)
            {
                Debug.WriteLine("Connecting!!!!!!!!!!!!!!!!!!!");
                if (Socket != null)
                    Socket.Dispose();
                if (Reader != null)
                    Reader.Dispose();
                if (Writer != null)
                    Writer.Dispose();

                Socket = null;
                Writer = null;
                Reader = null;

                _isBusy = false;

                while (Service.Device.ConnectionStatus != BluetoothConnectionStatus.Connected)
                {
                    try
                    {
                        await Connect();
                    }
                    catch
                    {
                        await Task.Delay(1000);
                    }
                }

                Unlock();
            }
        }

        public static async Task ConnectDevice()
        {
            if(Settings.PiCup == null)
            {
                await new DeviceConnectionDialog().ShowAsync();
            }
        }

        public async Task Connect()
        {
            if (Service.Device.ConnectionStatus == BluetoothConnectionStatus.Connected)
                return;

            try
            {
                Debug.WriteLine("Step 1");
                _isBusy = true;

                Socket = new StreamSocket();

                await Socket.ConnectAsync(Service.ConnectionHostName,
                    Service.ConnectionServiceName,
                    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                Debug.WriteLine("Step 2");
                Reader = new DataReader(Socket.InputStream);
                Writer = new DataWriter(Socket.OutputStream);


                Debug.WriteLine("Step 3");
                Reader.ByteOrder = ByteOrder.LittleEndian;
                Writer.ByteOrder = ByteOrder.LittleEndian;

                Unlock();
            }
            catch (Exception ee)
            {
                Socket = null;
                Reader = null;
                Writer = null;
                Debug.WriteLine($"Connection failed: '{ee.Message}'");

                throw new Exception($"Failed to connect: '{ee.Message}'");
            }
        }
        
        public async Task<List<float>> GetTemperature()
        {
            Lock();

            // send command
            byte[] msg = new byte[4];
            msg[0] = 17;

            Writer.WriteBytes(msg);
            await Writer.StoreAsync();

            // get result

            await Reader.LoadAsync(sizeof(float) * 3);
            var lst = new List<float>();
            for (int i = 0; i < 3; ++i)
                lst.Add(Reader.ReadSingle());

            Unlock();
            return lst;
        }

        public async Task SetLedState(int state)
        {
            Lock();

            byte[] msg = new byte[4];
            msg[0] = 16;
            msg[1] = (byte)state;

            Writer.WriteBytes(msg);
            await Writer.StoreAsync();

            Unlock();
        }

        public async Task<Measurement> GetMeasurement()
        {
            Lock();

            var meas = new Measurement();
            try
            {
                byte[] msg = new byte[4];
                msg[0] = 0x6;
                msg[1] = 5;
                msg[2] = 25;
                msg[3] = 255;

                Writer.WriteBytes(msg);
                await Writer.StoreAsync();
            }
            catch (Exception ee)
            {
                await new Windows.UI.Popups.MessageDialog($"Warning!\n{ee.Message}").ShowAsync();
                throw new Exception(ee.Message);
            }

            try
            {
                await Reader.LoadAsync(sizeof(float) * 36);

                var loc = new List<float>();
                for(int i = 0; i < 36; ++i)
                {
                    float value = Reader.ReadSingle();

                    meas.Data.Add(new SpectralData
                    {
                        Measurement = meas,
                        LedId = (int)(i / 18),
                        Channel = i,
                        Freq = SpectralData.GetChannelFreq(i % 18),
                        Value = value
                    });
                }
            }
            catch (Exception ee)
            {
                await new Windows.UI.Popups.MessageDialog($"Warning!\n{ee.Message}").ShowAsync();
                throw new Exception(ee.Message);
            }

            Unlock();
            return meas;
        }

        public async Task<List<float>> ReadSensors(int intTime = 20, int numObs = 1)
        {
            Lock();

            // send command
            byte[] msg = new byte[4];
            msg[0] = 18;
            msg[1] = (byte)intTime;
            msg[2] = (byte)numObs;

            Writer.WriteBytes(msg);
            await Writer.StoreAsync();

            // get result
            await Reader.LoadAsync(sizeof(float) * 18);
            var lst = new List<SpectralData>();
            for (int i = 0; i < 18; ++i)
            {
                var value = Reader.ReadSingle();
                Debug.WriteLine($"Channel {i}: {value}");
                lst.Add(new SpectralData
                {
                    Channel = i,
                    Freq = SpectralData.GetChannelFreq(i),
                    Value = value
                });
            }

            Unlock();
            return lst.OrderBy(a => a.Freq).Select(a=>a.Value).ToList();
        }

        void Lock()
        {
            while (_isBusy) ;
            _isBusy = true;
        }

        void Unlock()
        {
            _isBusy = false;
        }
    }
}
